using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CarX.Domain.Entities;
using CarX.Application.Interfaces;
using CarX.Infrastructure.Data;

// --- СЕРВИСНЫЙ СЛОЙ (Infrastructure) ---
namespace CarX.Infrastructure.Services
{
    public class BrandService : IBrandService
    {
        private readonly ApplicationDbContext _context;
        public BrandService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Brand>> GetAllAsync() 
        {
            // Используем AsNoTracking для GET-запросов (повышает производительность)
            return await _context.Brands.AsNoTracking().ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(long id) 
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<Brand> CreateAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<bool> UpdateAsync(Brand brand)
        {
            // Проверяем, существует ли сущность в базе перед обновлением
            var exists = await _context.Brands.AnyAsync(b => b.Id == brand.Id);
            if (!exists) return false;

            _context.Entry(brand).State = EntityState.Modified;
            
            try 
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;

            _context.Brands.Remove(brand);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}