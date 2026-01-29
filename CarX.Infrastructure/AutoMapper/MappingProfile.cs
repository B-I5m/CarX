// CarX.Application.Mappings/MappingProfile.cs
using AutoMapper;
using CarX.Application.Dtos;
using CarX.Domain.Entities;

namespace CarX.Infrastructure.AutoMapper;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Маппинг для списка (CarListItemDto)
        // Для списка
        CreateMap<Car, CarListItemDto>()
            .ConstructUsing(src => new CarListItemDto(
                src.Id,
                src.Model,
                src.Brand != null ? src.Brand.Name : "No Brand",
                src.Price,
                src.Year,
                src.MainImage, // Тут теперь MainImage
                src.Class.ToString()
            ));

// Для деталей
        CreateMap<Car, CarDetailDto>()
            .ConstructUsing(src => new CarDetailDto(
                src.Id,
                src.Model,
                src.Brand != null ? src.Brand.Name : "No Brand",
                src.Brand != null ? src.Brand.Country : string.Empty,
                src.Price,
                src.Year,
                src.MainImage, // И тут MainImage
                src.Class.ToString(),
                src.Brand != null ? src.Brand.BrandImage : null,
                src.Images.Select(i => i.ImagePath).ToList()
            ));
    }
}