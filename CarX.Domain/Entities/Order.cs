// CarX.Domain.Entities/Order.cs
namespace CarX.Domain.Entities;
using CarX.Domain.Common;
using CarX.Domain.Enums;

public class Order : BaseEntity
{
    public long UserId { get; set; }
    public virtual User? User { get; set; }

    public long CarId { get; set; }
    public virtual Car? Car { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.New;
    public decimal FinalPrice { get; set; } // Цена на момент покупки
}