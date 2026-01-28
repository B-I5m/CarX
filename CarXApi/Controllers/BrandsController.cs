using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using System.Collections.Generic;
using CarX.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _service;
    private readonly IWebHostEnvironment _env;

    public BrandsController(IBrandService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    // GET: api/brands
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    // GET: api/brands/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var brand = await _service.GetByIdAsync(id);
        if (brand == null) return NotFound("Бренд не найден");
        return Ok(brand);
    }

    // POST: api/brands
    [HttpPost]
    // [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] BrandCreateRequest request)
    {
        string fileName = await SaveImage(request.ImageFile);

        var brand = new Brand { 
            Name = request.Name, 
            Country = request.Country, 
            BrandImage = "/uploads/brands/" + fileName 
        };
        
        var created = await _service.CreateAsync(brand);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/brands/{id}
    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(long id, [FromBody] Brand brand)
    {
        if (id != brand.Id) return BadRequest("ID в URL и объекте не совпадают");

        var updated = await _service.UpdateAsync(brand);
        if (!updated) return NotFound("Бренд не найден для обновления");

        return NoContent();
    }

    // DELETE: api/brands/{id}
    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound("Бренд не найден");

        return Ok(new { message = "Бренд успешно удален" });
    }

    // Вспомогательный метод для сохранения картинки (по аналогии с Cars)
    private async Task<string> SaveImage(IFormFile? file)
    {
        // 1. Если файла нет, возвращаем дефолтную картинку
        if (file == null || file.Length == 0) return "default_brand.png";

        // 2. БЕЗОПАСНЫЙ ПУТЬ: если WebRootPath null, берем путь вручную
        var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    
        var directoryPath = Path.Combine(rootPath, "uploads", "brands");

        // 3. Создаем папку, если её физически нет
        if (!Directory.Exists(directoryPath)) 
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 4. Генерируем уникальное имя файла
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(directoryPath, fileName);

        // 5. Сохраняем файл
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }
}