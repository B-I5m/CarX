// CarX.Domain.Entities/CarImage.cs
namespace CarX.Domain.Entities;
using CarX.Domain.Common;

public class CarImage : BaseEntity
{
    public string ImagePath { get; set; } = string.Empty;
    public long CarId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Car? Car { get; set; }
}