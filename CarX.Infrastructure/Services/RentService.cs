// CarX.Infrastructure.Services/RentService.cs

using CarX.Application.Dtos;
using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class RentService : IRentService
{
    private readonly ApplicationDbContext _context;
    public RentService(ApplicationDbContext context) => _context = context;

    public async Task<Rent> CreateRentAsync(RentCreateRequest request)
    {
        var car = await _context.Cars.FindAsync(request.CarId) ?? throw new Exception("Car not found");
        var days = (request.ToDate.Date - request.FromDate.Date).Days;
        if (days <= 0) days = 1;

        var rent = new Rent {
            CarId = request.CarId, UserId = request.UserId,
            FromDate = request.FromDate, ToDate = request.ToDate,
            TotalPrice = car.Price * days, Status = RentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.Rents.Add(rent);
        await _context.SaveChangesAsync();
        return rent;
    }

    public async Task<IEnumerable<Rent>> GetAllRentsAsync() => 
        await _context.Rents.Include(r => r.Car).Include(r => r.User).AsNoTracking().ToListAsync();

    // CarX.Infrastructure.Services/RentService.cs
    public async Task<Rent?> GetByIdAsync(long id) // Исправил название
    {
        return await _context.Rents
            .Include(r => r.Car)
            .Include(r => r.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateRentStatusAsync(long id, RentStatus newStatus)
    {
        var rent = await _context.Rents.FindAsync(id);
        if (rent == null) return false;
        rent.Status = newStatus;
        rent.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteRentAsync(long id)
    {
        var rent = await _context.Rents.FindAsync(id);
        if (rent == null) return false;
        _context.Rents.Remove(rent);
        return await _context.SaveChangesAsync() > 0;
    }
}