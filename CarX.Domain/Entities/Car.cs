using CarX.Domain.Enums;

namespace CarX.Domain.Entities;
    
// CarX.Domain.Entities/Car.cs
public class Car
{
    public long Id { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
    public CarClass Class { get; set; } 
    public string CarImage { get; set; } 

    public long BrandId { get; set; }
    public virtual Brand Brand { get; set; }
    public virtual List<CarImage> CarImages { get; set; }
}