using CarX.Domain.Entities;
using CarX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarX.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly ApplicationDbContext _context;

    public FavoriteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> ToggleFavoriteAsync(long userId, long? carId, long? rentCarId)
    {
        var existing = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && 
                                      f.CarId == carId && 
                                      f.RentCarId == rentCarId);

        if (existing != null)
        {
            _context.Favorites.Remove(existing);
            await _context.SaveChangesAsync();
            return "Removed";
        }

        var favorite = new Favorite
        {
            UserId = userId,
            CarId = carId,
            RentCarId = rentCarId
        };

        await _context.Favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();
        return "Added";
    }

    public async Task<IEnumerable<object>> GetUserFavoritesAsync(long userId)
    {
        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Car).ThenInclude(c => c!.Brand)
            .Include(f => f.RentCar).ThenInclude(rc => rc!.Brand)
            .Include(f => f.RentCar).ThenInclude(rc => rc!.Images)
            .ToListAsync();

        return favorites.Select(f => 
        {
            if (f.RentCarId != null)
            {
                return (object)new {
                    Id = f.Id,
                    Type = "Rent",
                    CarId = f.RentCarId,
                    Model = f.RentCar?.Model,
                    Brand = f.RentCar?.Brand?.Name,
                    Price = f.RentCar?.PricePerDay,
                    Image = f.RentCar?.Images?.FirstOrDefault()?.Url 
                };
            }
        
            return (object)new {
                Id = f.Id,
                Type = "Sale",
                CarId = f.CarId,
                Model = f.Car?.Model,
                Brand = f.Car?.Brand?.Name,
                Price = f.Car?.Price,
                Image = "" // Заглушка для фото обычной машины
            };
        }).ToList(); 
    }
    public async Task<bool> RemoveCarFavoriteAsync(long userId, long carId)
    {
        var fav = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.CarId == carId);
        if (fav == null) return false;
    
        _context.Favorites.Remove(fav);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveRentCarFavoriteAsync(long userId, long rentCarId)
    {
        var fav = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.RentCarId == rentCarId);
        if (fav == null) return false;

        _context.Favorites.Remove(fav);
        return await _context.SaveChangesAsync() > 0;
    }
}