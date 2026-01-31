using CarX.Domain.Entities;

namespace CarX.Domain.Entities;
public class Favorite
{
    public long Id { get; set; }
    
    public long UserId { get; set; }
    public virtual User User { get; set; } = null!;

    // Ссылка на обычную машину (может быть null)
    public long? CarId { get; set; }
    public virtual Car? Car { get; set; }

    // Ссылка на машину в аренду (может быть null)
    public long? RentCarId { get; set; }
    public virtual RentCar? RentCar { get; set; }
}