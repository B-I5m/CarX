using CarX.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CarX.Application.Dtos;

public class CarCreateRequest
{
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public CarClass Class { get; set; }
    public long BrandId { get; set; }
    public IFormFile? ImageFile { get; set; }
    
    public ICollection<IFormFile>? ImageFiles { get; set; }
}