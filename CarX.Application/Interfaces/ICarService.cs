using System.Collections.Generic;
using System.Threading.Tasks;
using CarX.Domain.Entities;
using CarX.Domain.Enums;

using System.Collections.Generic;
using System.Threading.Tasks;
using CarX.Domain.Entities;
using CarX.Domain.Enums;

namespace CarX.Application.Interfaces;

public interface ICarService
{
  
    Task<IEnumerable<Car>> GetFilteredCarsAsync(string? searchTerm, decimal? minPrice, decimal? maxPrice, CarClass? carClass);
    
    Task<Car?> GetByIdAsync(long id);
    Task<Car> CreateAsync(Car car);
    Task<bool> UpdateAsync(Car car);
    Task<bool> DeleteAsync(long id);
}