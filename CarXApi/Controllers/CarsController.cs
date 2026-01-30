using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CarX.Application.Interfaces;
using CarX.Application.Dtos;
using CarX.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CarXWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _service;
        private readonly IMapper _mapper;

        public CarsController(ICarService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // 1. ПОЛУЧИТЬ ВСЕ (с базовой фильтрацией: поиск, цена, класс)
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? search, 
            [FromQuery] decimal? min, 
            [FromQuery] decimal? max, 
            [FromQuery] CarClass? cls)
        {
            // Вызываем метод из сервиса
            var cars = await _service.GetFilteredCarsAsync(search, min, max, cls);
            
            // Маппим результат в список для отображения
            var dtos = _mapper.Map<IEnumerable<CarListItemDto>>(cars);
            
            return Ok(dtos);
        }

        // 2. УМНЫЙ ПОДБОР (Match) — поиск по всем тех. характеристикам
        [HttpGet("match")]
        public async Task<IActionResult> FindMatch([FromQuery] CarMatchRequest request)
        {
            // Используем специальный метод сервиса для глубокого поиска
            var cars = await _service.FindSuitableCarsAsync(request);
    
            if (cars == null) 
                return NotFound("Подходящих машин не найдено.");

            var dtos = _mapper.Map<IEnumerable<CarListItemDto>>(cars);
            return Ok(dtos);
        }

        // 3. ПОЛУЧИТЬ ПО ID (Детальная информация с галереей)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var car = await _service.GetByIdAsync(id);

            if (car == null) 
                return NotFound("Машина не найдена");
    
            // Маппим в подробное DTO (с цветом, лошадьми, разгоном и т.д.)
            var dto = _mapper.Map<CarDetailDto>(car);
            
            return Ok(dto);
        }

        // 4. СОЗДАТЬ МАШИНУ (Принимает форму с файлами изображений)
        [HttpPost]
        [Consumes("multipart/form-data")]
        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CarCreateRequest request)
        {
            var car = await _service.CreateAsync(request);
    
            // ВМЕСТО return Ok(car); 
            // ДЕЛАЙ ТАК:
            var dto = _mapper.Map<CarDetailDto>(car); 
            return CreatedAtAction(nameof(GetById), new { id = car.Id }, dto);
        }

        // 5. ОБНОВИТЬ МАШИНУ (Полное обновление данных и главного фото)
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(long id, [FromForm] CarCreateRequest request) 
        {
            // Передаем ID и DTO в сервис
            var updated = await _service.UpdateAsync(id, request);
            
            if (!updated) 
                return NotFound("Машина не найдена или не удалось обновить данные.");

            return Ok(new { message = "Данные машины успешно обновлены" });
        }

        // 6. УДАЛИТЬ МАШИНУ
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _service.DeleteAsync(id);
            
            if (!deleted) 
                return NotFound("Машина не найдена");
            
            return Ok(new { message = "Машина успешно удалена из системы" });
        }
    }
}