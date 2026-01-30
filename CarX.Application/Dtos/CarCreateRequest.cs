// CarX.Application.Dtos/CarCreateRequest.cs

using CarX.Domain.Enums;
using Microsoft.AspNetCore.Http;

public class CarCreateRequest
{
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Color { get; set; } = string.Empty;
    public int Horsepower { get; set; }
    public double TrunkVolume { get; set; }
    public double Weight { get; set; }
    public double EngineVolume { get; set; }
    public double Acceleration0to100 { get; set; }
    public TransmissionType Transmission { get; set; }
    public BodyType BodyType { get; set; }
    public CarClass Class { get; set; }
    public long BrandId { get; set; }

    // Файлы
    public IFormFile? ImageFile { get; set; } // Главное фото
    public ICollection<IFormFile>? ImageFiles { get; set; } // Доп. фото
}