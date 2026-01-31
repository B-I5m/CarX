using Microsoft.AspNetCore.Http;

public interface IRentCarService
{
    // Было: Task<RentCar> CreateAsync(RentCar car, IFormFile image);
    // СТАЛО:
    Task<RentCar> CreateAsync(RentCar car, List<IFormFile> images);
    
    // Остальные методы...
    Task<IEnumerable<RentCar>> GetAllAsync();
    Task<RentCar?> GetByIdAsync(long id);
    Task<bool> UpdateAsync(long id, RentCar car);
    Task<bool> DeleteAsync(long id);
}