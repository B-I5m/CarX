// CarX.Domain.Enums/OrderStatus.cs
namespace CarX.Domain.Enums;

public enum OrderStatus
{
    New,            // Новая заявка
    InProcess,      // В обработке
    Confirmed,      // Подтвержден
    Delivered,      // Доставлен/Завершен
    Cancelled       // Отменен
}