// CarX.Application.Interfaces/IRentService.cs

using CarX.Application.Dtos;
using CarX.Domain.Entities;
using CarX.Domain.Enums;

public interface IRentService
{
    Task<Rent> CreateRentAsync(RentCreateRequest request);
    Task<IEnumerable<Rent>> GetAllRentsAsync();
    Task<Rent?> GetByIdAsync(long id); // Называем просто GetByIdAsync
    Task<bool> UpdateRentStatusAsync(long id, RentStatus newStatus);
    Task<bool> DeleteRentAsync(long id);
}