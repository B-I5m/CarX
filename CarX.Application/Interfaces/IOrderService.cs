// CarX.Application.Interfaces/IOrderService.cs
using CarX.Domain.Entities;
using CarX.Domain.Enums;

namespace CarX.Application.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(long carId, long userId);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(long id);
    Task<bool> UpdateOrderStatusAsync(long id, OrderStatus newStatus);
    Task<bool> DeleteOrderAsync(long id);
}