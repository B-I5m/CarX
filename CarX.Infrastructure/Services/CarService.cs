using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarX.Infrastructure.Data;

namespace CarX.Infrastructure.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ТОТ САМЫЙ МЕТОД ФИЛЬТРАЦИИ
        // В файле CarService.cs
        public async Task<IEnumerable<Car>> GetFilteredCarsAsync(string? searchTerm, decimal? minPrice, decimal? maxPrice, CarClass? carClass)
        {
            var query = _context.Cars
                .Include(c => c.Brand)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();
                query = query.Where(x => x.Model.ToLower().Contains(lowerSearch) || 
                                         x.Brand.Name.ToLower().Contains(lowerSearch));
            }

            if (minPrice.HasValue) query = query.Where(x => x.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(x => x.Price <= maxPrice.Value);
            if (carClass.HasValue) query = query.Where(x => x.Class == carClass.Value);

            return await query.ToListAsync();
        }

        

        public async Task<Car?> GetByIdAsync(long id)
        {
            return await _context.Cars
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Car> CreateAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<bool> UpdateAsync(Car car)
        {
            var existing = await _context.Cars.FindAsync(car.Id);
            if (existing == null) return false;

            // Обновляем значения
            _context.Entry(existing).CurrentValues.SetValues(car);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return false;

            _context.Cars.Remove(car);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
