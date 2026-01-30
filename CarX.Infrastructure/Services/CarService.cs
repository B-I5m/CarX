// CarX.Infrastructure.Services/CarService.cs
using CarX.Application.Dtos;
using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarX.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CarX.Infrastructure.Services;

public class CarService : ICarService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CarService(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // --- МЕТОДЫ ПОИСКА ---

    public async Task<IEnumerable<Car>> GetFilteredCarsAsync(string? searchTerm, decimal? minPrice, decimal? maxPrice, CarClass? carClass)
    {
        var query = _context.Cars
            .Include(c => c.Brand)
            .Include(c => c.Images)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lower = searchTerm.ToLower();
            query = query.Where(x => x.Model.ToLower().Contains(lower) || x.Brand.Name.ToLower().Contains(lower));
        }

        if (minPrice.HasValue) query = query.Where(x => x.Price >= minPrice);
        if (maxPrice.HasValue) query = query.Where(x => x.Price <= maxPrice);
        if (carClass.HasValue) query = query.Where(x => x.Class == carClass);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Car>> FindSuitableCarsAsync(CarMatchRequest request)
    {
        var query = _context.Cars
            .Include(c => c.Brand)
            .Include(c => c.Images)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(request.Model))
            query = query.Where(c => c.Model.ToLower().Contains(request.Model.ToLower()) 
                                     || c.Brand.Name.ToLower().Contains(request.Model.ToLower()));

        if (!string.IsNullOrEmpty(request.Color))
            query = query.Where(c => c.Color.ToLower().Contains(request.Color.ToLower()));

        if (request.MaxPrice.HasValue) query = query.Where(c => c.Price <= request.MaxPrice.Value);
        if (request.MinYear.HasValue) query = query.Where(c => c.Year >= request.MinYear.Value);
        if (request.MinHorsepower.HasValue) query = query.Where(c => c.Horsepower >= request.MinHorsepower.Value);
        if (request.MinEngineVolume.HasValue) query = query.Where(c => c.EngineVolume >= request.MinEngineVolume.Value);
        if (request.MaxAcceleration.HasValue) query = query.Where(c => c.Acceleration0to100 <= request.MaxAcceleration.Value);
        if (request.Transmission.HasValue) query = query.Where(c => c.Transmission == request.Transmission.Value);
        if (request.BodyType.HasValue) query = query.Where(c => c.BodyType == request.BodyType.Value);

        return await query.ToListAsync();
    }

    // --- CRUD ОПЕРАЦИИ ---

    public async Task<Car?> GetByIdAsync(long id)
    {
        return await _context.Cars
            .Include(c => c.Brand)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Car> CreateAsync(CarCreateRequest request)
    {
        var car = new Car
        {
            Model = request.Model,
            Year = request.Year,
            Price = request.Price,
            Color = request.Color,
            Horsepower = request.Horsepower,
            EngineVolume = request.EngineVolume,
            Acceleration0to100 = request.Acceleration0to100,
            Transmission = request.Transmission,
            BodyType = request.BodyType,
            Class = request.Class,
            BrandId = request.BrandId,
            MainImage = "/uploads/cars/" + await SaveImage(request.ImageFile)
        };

        if (request.ImageFiles != null)
        {
            foreach (var file in request.ImageFiles)
            {
                var fileName = await SaveImage(file);
                car.Images.Add(new CarImage { ImagePath = "/uploads/cars/" + fileName });
            }
        }

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();
        return car;
    }

    public async Task<bool> UpdateAsync(long id, CarCreateRequest request)
    {
        var car = await _context.Cars.Include(c => c.Images).FirstOrDefaultAsync(x => x.Id == id);
        if (car == null) return false;

        car.Model = request.Model;
        car.Year = request.Year;
        car.Price = request.Price;
        car.Color = request.Color;
        car.Horsepower = request.Horsepower;
        car.EngineVolume = request.EngineVolume;
        car.Acceleration0to100 = request.Acceleration0to100;
        car.Transmission = request.Transmission;
        car.BodyType = request.BodyType;
        car.Class = request.Class;
        car.BrandId = request.BrandId;

        if (request.ImageFile != null)
            car.MainImage = "/uploads/cars/" + await SaveImage(request.ImageFile);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return false;

        _context.Cars.Remove(car);
        return await _context.SaveChangesAsync() > 0;
    }

    // --- ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ФАЙЛОВ ---

    private async Task<string> SaveImage(IFormFile? file)
    {
        if (file == null || file.Length == 0) return "default_car.png";
        
        var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var directoryPath = Path.Combine(rootPath, "uploads", "cars");
        
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(directoryPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return fileName;
    }
}