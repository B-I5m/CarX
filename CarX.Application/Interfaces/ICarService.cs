// CarX.Application.Interfaces/ICarService.cs
using CarX.Application.Dtos;
using CarX.Domain.Entities;
using CarX.Domain.Enums;

namespace CarX.Application.Interfaces;

public interface ICarService
{
    // Обычный поиск (для списка)
    Task<IEnumerable<Car>> GetFilteredCarsAsync(string? searchTerm, decimal? minPrice, decimal? maxPrice, CarClass? carClass);
    
    // Умный поиск (тот самый "Match")
    Task<IEnumerable<Car>> FindSuitableCarsAsync(CarMatchRequest request);
    
    Task<Car?> GetByIdAsync(long id);
    Task<Car> CreateAsync(CarCreateRequest request);
    Task<bool> UpdateAsync(long id, CarCreateRequest request);
    Task<bool> DeleteAsync(long id);
}