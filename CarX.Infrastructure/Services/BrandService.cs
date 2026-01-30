using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarX.Application.Dtos;
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

        // CarX.Infrastructure.Services/BrandService.cs

        // CarX.Infrastructure.Services/BrandService.cs

        public async Task<bool> UpdateBrandAsync(long id, BrandUpdateRequest request)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;

            brand.Name = request.Name;
            brand.Country = request.Country;

            // ЛОГИКА ДЛЯ ФАЙЛА:
            if (request.BrandImage != null)
            {
                // 1. Генерируем имя файла (например: brand_1.jpg)
                string fileName = $"brand_{id}{Path.GetExtension(request.BrandImage.FileName)}";
        
                // 2. Путь, куда сохранить (в папку wwwroot/images)
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, fileName);

                // 3. Сохраняем файл физически на диск
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.BrandImage.CopyToAsync(stream);
                }

                // 4. В базу записываем только ПУТЬ (строку)
                brand.BrandImage = $"/images/{fileName}";
            }

            return await _context.SaveChangesAsync() > 0;
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