using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarXWebApi.Application.Dtos; // Убедись, что этот namespace верный
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _service;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public CarsController(ICarService service, IWebHostEnvironment env, IMapper mapper)
    {
        _service = service;
        _env = env;
        _mapper = mapper;
    }

    // 1. ПОЛУЧИТЬ ВСЕ (с фильтрацией)
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? search,   // Теперь контроллер видит слово "search" из URL
        [FromQuery] decimal? min, 
        [FromQuery] decimal? max, 
        [FromQuery] CarClass? cls)
    {
        // Теперь переменная 'search' передается первой, как мы и прописали в интерфейсе
        var cars = await _service.GetFilteredCarsAsync(search, min, max, cls);
    
        var dtos = _mapper.Map<IEnumerable<CarListItemDto>>(cars);
        return Ok(dtos);
    }
    

    // 2. ПОЛУЧИТЬ ПО ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var car = await _service.GetByIdAsync(id);
        if (car == null) return NotFound("Машина не найдена");
        
        var dto = _mapper.Map<CarDetailDto>(car);
        return Ok(dto);
    }

    // 3. СОЗДАТЬ МАШИНУ
    [HttpPost]
    // [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CarCreateRequest request)
    {
        string fileName = await SaveImage(request.ImageFile);
        

        var car = new Car {
            Model = request.Model,
            Year = request.Year,
            Price = request.Price,
            Class = request.Class,
            BrandId = request.BrandId,
            CarImage = "/uploads/cars/" + fileName
        };
        
        

        var created = await _service.CreateAsync(car);
        var dto = _mapper.Map<CarDetailDto>(created);
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
    }

    // 4. ОБНОВИТЬ МАШИНУ
    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(long id, [FromBody] CarCreateRequest request) // Используем Request или DTO
    {
        var existingCar = await _service.GetByIdAsync(id);
        if (existingCar == null) return NotFound("Машина не найдена");

        // Обновляем поля
        existingCar.Model = request.Model;
        existingCar.Year = request.Year;
        existingCar.Price = request.Price;
        existingCar.Class = request.Class;
        existingCar.BrandId = request.BrandId;

        // Если пришло новое фото — обновляем, если нет — оставляем старое
        if (request.ImageFile != null)
        {
            existingCar.CarImage = "/uploads/cars/" + await SaveImage(request.ImageFile);
        }

        var result = await _service.UpdateAsync(existingCar);
        if (!result) return BadRequest("Не удалось обновить данные");

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