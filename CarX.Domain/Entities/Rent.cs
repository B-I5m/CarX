// CarX.Domain.Entities/Rent.cs
namespace CarX.Domain.Entities;
using CarX.Domain.Common;
using CarX.Domain.Enums;

public class Rent : BaseEntity
{
    public long UserId { get; set; }
    public virtual User? User { get; set; }

    public long CarId { get; set; }
    public virtual Car? Car { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalPrice { get; set; }
    public RentStatus Status { get; set; } = RentStatus.Pending;
}