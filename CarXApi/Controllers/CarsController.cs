using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarX.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CarX.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    // CarsController.cs
    private readonly ICarService _service;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context; // Добавь это поле

    public CarsController(ICarService service, IWebHostEnvironment env, IMapper mapper, ApplicationDbContext context)
    {
        _service = service;
        _env = env;
        _mapper = mapper;
        _context = context; // И это
    }

    // 1. ПОЛУЧИТЬ ВСЕ (с фильтрацией)
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? search, 
        [FromQuery] decimal? min, 
        [FromQuery] decimal? max, 
        [FromQuery] CarClass? cls)
    {
        // Сервис вернет список, даже если фильтры пустые (null)
        var cars = await _service.GetFilteredCarsAsync(search, min, max, cls);

        // Теперь CarListItemDto существует и маппер его увидит
        var dtos = _mapper.Map<IEnumerable<CarListItemDto>>(cars);
        return Ok(dtos);
    }

// 2. ПОЛУЧИТЬ ПО ID (с галереей)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        // Важно: в сервисе или тут через контекст нужно сделать Include(c => c.Images)
        var car = await _context.Cars
            .Include(c => c.Brand)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (car == null) return NotFound("Машина не найдена");
    
        var dto = _mapper.Map<CarDetailDto>(car);
        return Ok(dto);
    }

    // Вставь это в CarsController.cs вместо старых методов

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CarCreateRequest request)
    {
        // 1. Сохраняем главное фото
        string mainImgName = await SaveImage(request.ImageFile);

        var car = new Car {
            Model = request.Model,
            Year = request.Year,
            Price = request.Price,
            Class = request.Class,
            BrandId = request.BrandId,
            MainImage = "/uploads/cars/" + mainImgName, // Используем MainImage
            Images = new List<CarImage>() 
        };

        // 2. Сохраняем дополнительные фото
        if (request.ImageFiles != null)
        {
            foreach (var file in request.ImageFiles)
            {
                string fileName = await SaveImage(file);
                car.Images.Add(new CarImage { ImagePath = "/uploads/cars/" + fileName });
            }
        }

        await _context.Cars.AddAsync(car);
        await _context.SaveChangesAsync();
    
        return Ok(car);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromForm] CarCreateRequest request) 
    {
        var existingCar = await _context.Cars.Include(c => c.Images).FirstOrDefaultAsync(x => x.Id == id);
        if (existingCar == null) return NotFound("Машина не найдена");

        existingCar.Model = request.Model;
        existingCar.Year = request.Year;
        existingCar.Price = request.Price;
        existingCar.Class = request.Class;
        existingCar.BrandId = request.BrandId;

        if (request.ImageFile != null)
        {
            // Исправлено: CarImage заменен на MainImage
            existingCar.MainImage = "/uploads/cars/" + await SaveImage(request.ImageFile);
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
    // 5. УДАЛИТЬ МАШИНУ
    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound("Машина не найдена");
        
        return Ok(new { message = "Машина успешно удалена" });
    }

    // --- ПРИВАТНЫЕ МЕТОДЫ ---

    private async Task<string> SaveImage(IFormFile? file)
    {
        if (file == null || file.Length == 0) return "default_car.png";
        
        var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var directoryPath = Path.Combine(rootPath, "uploads", "cars");
        
        if (!Directory.Exists(directoryPath)) 
            Directory.CreateDirectory(directoryPath);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(directoryPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create)) 
        { 
            await file.CopyToAsync(stream); 
        }
        
        return fileName;
    }
}