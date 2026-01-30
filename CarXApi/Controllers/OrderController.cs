// CarXWebApi/Controllers/OrdersController.cs

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using CarX.Application.Interfaces;
using CarX.Application.Dtos;
using CarX.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] OrderCreateRequest request)
    {
        try {
            var order = await _orderService.CreateOrderAsync(request.CarId, request.UserId);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        } catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll() => Ok(await _orderService.GetAllOrdersAsync());

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(long id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] OrderStatus status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, status);
        return result ? Ok(new { message = "Статус обновлен" }) : NotFound();
    }
    

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        // 1. Получаем ID юзера, который сейчас авторизован
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
    
        long currentUserId = long.Parse(userIdString);

        // 2. Ищем заказ в базе
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound("Заказ не найден");

        // 3. ПРОВЕРКА: Если это не Админ И не владелец заказа — шлем лесом
        bool isAdmin = User.IsInRole("Admin");
        if (!isAdmin && order.UserId != currentUserId)
        {
            return Forbid(); // Ошибка 403: Доступ запрещен (это не твой заказ)
        }

        // 4. Если проверка прошла, удаляем
        var result = await _orderService.DeleteOrderAsync(id);
        return result ? Ok(new { message = "Заказ удален" }) : NotFound();
    }
}