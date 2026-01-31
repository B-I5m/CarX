using CarX.Application.Dtos;
using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarX.Infrastructure.Services;

public class RentService : IRentService
{
    private readonly ApplicationDbContext _context;

    public RentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rent> CreateRentAsync(RentCreateRequest request)
    {
        // 1. Ищем машину именно в таблице машин для аренды
        var car = await _context.RentCars.FindAsync(request.RentCarId);
        if (car == null) 
            throw new Exception("Ошибка: Машина для аренды не найдена в базе.");

        // 2. Считаем количество дней аренды
        int totalDays = (request.ToDate.Date - request.FromDate.Date).Days;
        if (totalDays <= 0) totalDays = 1; // Минимум 1 день

        // 3. Создаем объект заказа на аренду
        var rent = new Rent
        {
            RentCarId = request.RentCarId,
            UserId = request.UserId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            // АВТОМАТИЧЕСКИЙ РАСЧЕТ: цена за день * кол-во дней
            TotalPrice = car.PricePerDay * totalDays, 
            Status = RentStatus.Pending
        };

        // 4. Сохраняем в базу
        _context.Rents.Add(rent);
        await _context.SaveChangesAsync();

        return rent;
    }

    public async Task<IEnumerable<Rent>> GetAllRentsAsync() => 
        await _context.Rents
            .Include(r => r.RentCar)
            .Include(r => r.User)
            .ToListAsync();

    public async Task<Rent?> GetByIdAsync(long id) => 
        await _context.Rents
            .Include(r => r.RentCar)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<bool> UpdateRentStatusAsync(long id, RentStatus newStatus)
    {
        var rent = await _context.Rents.FindAsync(id);
        if (rent == null) return false;

        rent.Status = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRentAsync(long id)
    {
        var rent = await _context.Rents.FindAsync(id);
        if (rent == null) return false;

        _context.Rents.Remove(rent);
        await _context.SaveChangesAsync();
        return true;
    }
}