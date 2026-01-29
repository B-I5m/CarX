// CarXWebApi/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using CarX.Application.Interfaces;
using CarX.Application.Dtos;
using CarX.Domain.Enums;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderCreateRequest request)
    {
        try {
            var order = await _orderService.CreateOrderAsync(request.CarId, request.UserId);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        } catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _orderService.GetAllOrdersAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] OrderStatus status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, status);
        return result ? Ok(new { message = "Статус обновлен" }) : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        return result ? Ok(new { message = "Заказ удален" }) : NotFound();
    }
}