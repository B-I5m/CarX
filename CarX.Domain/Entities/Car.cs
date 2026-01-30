// CarX.Domain.Entities/Car.cs
namespace CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarX.Domain.Common;

public class Car : BaseEntity
{
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string MainImage { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty; // Цвет (строкой, чтобы можно было писать "Cosmic Blue")

    // Технические характеристики
    public int Horsepower { get; set; }       // Лошадиные силы
    public double EngineVolume { get; set; }  // Объем двигателя (например, 2.5)
    public double Acceleration0to100 { get; set; } // Разгон до 100 км/ч (сек)
    public TransmissionType Transmission { get; set; } // Коробка
    public BodyType BodyType { get; set; }    // Кузов
    
    // Старые поля
    public CarClass Class { get; set; } 
    public long BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
    public virtual ICollection<CarImage> Images { get; set; } = new List<CarImage>();
}