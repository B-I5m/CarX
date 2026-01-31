// CarX.Domain.Entities/RentCarImage.cs
using CarX.Domain.Common;

namespace CarX.Domain.Entities;

public class RentCarImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public long RentCarId { get; set; }
    public virtual RentCar? RentCar { get; set; }
}