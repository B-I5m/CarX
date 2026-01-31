// CarX.Domain.Entities/RentCar.cs

using CarX.Domain.Common;
using CarX.Domain.Entities;

public class RentCar : BaseEntity
{
    public string Model { get; set; } = string.Empty;
    public long BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal Deposit { get; set; }
    public bool IsBusy { get; set; }
    public int Year { get; set; }
    public string Transmission { get; set; } = string.Empty;

    // Коллекция картинок
    public virtual ICollection<RentCarImage> Images { get; set; } = new List<RentCarImage>();
}