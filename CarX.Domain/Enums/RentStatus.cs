// CarX.Domain.Enums/RentStatus.cs
namespace CarX.Domain.Enums;

public enum RentStatus
{
    Pending,        // Ожидает подтверждения
    Active,         // Машина у клиента
    Completed,      // Возвращена
    Overdue,        // Просрочена
    Cancelled       // Отменена
}