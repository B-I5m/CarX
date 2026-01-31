using CarX.Domain.Entities;
using CarX.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CarX.Infrastructure.Services;

public class RentCarService : IRentCarService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public RentCarService(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // Возвращаем ТОЛЬКО доступные машины (для витрины)
    public async Task<IEnumerable<RentCar>> GetAllAvailableAsync() => 
        await _context.RentCars
            .Include(c => c.Brand)
            .Include(c => c.Images)
            .Where(c => !c.IsBusy) 
            .ToListAsync();

    // Админ видит всё (и занятые, и свободные)
    public async Task<IEnumerable<RentCar>> GetAllAsync() => 
        await _context.RentCars
            .Include(c => c.Brand)
            .Include(c => c.Images)
            .ToListAsync();

    public async Task<RentCar?> GetByIdAsync(long id)
    {
        return await _context.RentCars
            .Include(c => c.Brand)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<RentCar>> GetByBrandAsync(long brandId) =>
        await _context.RentCars
            .Where(c => c.BrandId == brandId && !c.IsBusy)
            .Include(c => c.Brand)
            .Include(c => c.Images)
            .ToListAsync();

    public async Task<RentCar> CreateAsync(RentCar car, List<IFormFile> images)
    {
        if (images != null && images.Any())
        {
            foreach (var file in images)
            {
                var filePath = await SaveImage(file);
                car.Images.Add(new RentCarImage { Url = filePath });
            }
        }

        _context.RentCars.Add(car);
        await _context.SaveChangesAsync();
        return car;
    }

    public async Task<bool> UpdateAsync(long id, RentCar car)
    {
        var existingCar = await _context.RentCars
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (existingCar == null) return false;

        // Обновляем поля
        existingCar.Model = car.Model;
        existingCar.BrandId = car.BrandId;
        existingCar.PricePerDay = car.PricePerDay;
        existingCar.Deposit = car.Deposit;
        existingCar.IsBusy = car.IsBusy;
        existingCar.Year = car.Year;
        existingCar.Transmission = car.Transmission;

        // Обновление картинок: если прислали новые — заменяем полностью
        if (car.Images != null && car.Images.Any())
        {
            // Можно добавить логику удаления старых файлов с диска здесь
            existingCar.Images.Clear();
            foreach (var img in car.Images)
            {
                existingCar.Images.Add(img);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var car = await _context.RentCars
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car == null) return false;

        // Удаляем файлы с физического диска, чтобы не забивать память
        if (car.Images != null)
        {
            foreach (var image in car.Images)
            {
                var fullPath = Path.Combine(_env.WebRootPath, image.Url.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }

        _context.RentCars.Remove(car);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsBusyAsync(long carId)
    {
        var car = await _context.RentCars.FindAsync(carId);
        if (car == null) return false;

        car.IsBusy = true;
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> SaveImage(IFormFile image)
    {
        // Используем _env.WebRootPath — это путь к wwwroot
        var folderPath = Path.Combine(_env.WebRootPath, "uploads", "rent-cars");
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var fullPath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return $"/uploads/rent-cars/{fileName}";
    }
}