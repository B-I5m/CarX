using System.Collections.Generic;

namespace CarX.Domain.Entities;

public class Brand
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; } // Добавили страну
    public string BrandImage { get; set; } // Тут будет лежать путь к файлу (напр. /images/brands/bmw.png)
    
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}