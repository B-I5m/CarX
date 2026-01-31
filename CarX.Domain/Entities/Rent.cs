// CarX.Domain.Entities/Rent.cs

using CarX.Domain.Common;
using CarX.Domain.Entities;
using CarX.Domain.Enums;

namespace CarX.Domain.Entities;
public class Rent : BaseEntity
{
    public long UserId { get; set; }
    public virtual User? User { get; set; }

    // МЕНЯЕМ ТУТ: ссылка на RentCarId вместо CarId
    public long RentCarId { get; set; } 
    public virtual RentCar? RentCar { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalPrice { get; set; }
    public RentStatus Status { get; set; } = RentStatus.Pending;
}