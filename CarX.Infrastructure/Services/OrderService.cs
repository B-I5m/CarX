// CarX.Infrastructure.Services/OrderService.cs
using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarX.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context) => _context = context;

    public async Task<Order> CreateOrderAsync(long carId, long userId)
    {
        var car = await _context.Cars.FindAsync(carId) 
                  ?? throw new Exception("Машина не найдена");

        var order = new Order
        {
            CarId = carId,
            UserId = userId,
            FinalPrice = car.Price,
            Status = OrderStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Car)
            .Include(o => o.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(long id)
    {
        return await _context.Orders
            .Include(o => o.Car)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<bool> UpdateOrderStatusAsync(long id, OrderStatus newStatus)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow; // Используем метод из BaseEntity или пишем вручную
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteOrderAsync(long id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        return await _context.SaveChangesAsync() > 0;
    }
}