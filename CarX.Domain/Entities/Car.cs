// CarX.Domain.Entities/Car.cs (Обновленный)
namespace CarX.Domain.Entities;
using CarX.Domain.Enums;
using CarX.Domain.Common;

public class Car : BaseEntity
{
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; } // Для продажи — полная цена, для аренды — цена в сутки
    public CarClass Class { get; set; } 
    public string MainImage { get; set; } = string.Empty; // Оставляем одну главной

    public long BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
    
    // Коллекция доп. изображений
    public virtual ICollection<CarImage> Images { get; set; } = new List<CarImage>();
}