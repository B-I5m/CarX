using AutoMapper;
using CarX.Application.Dtos;
using CarX.Domain.Entities;
using System.Linq;

namespace CarX.Infrastructure.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 1. МАППИНГ ДЛЯ ДЕТАЛЬНОЙ ИНФОРМАЦИИ (CarDetailDto - RECORD)
        // Для рекордов используем ForCtorParam, так как у них нет пустого конструктора
        CreateMap<Car, CarDetailDto>()
            .ForCtorParam("BrandName", opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : "No Brand"))
            .ForCtorParam("Transmission", opt => opt.MapFrom(src => src.Transmission.ToString()))
            .ForCtorParam("BodyType", opt => opt.MapFrom(src => src.BodyType.ToString()))
            .ForCtorParam("AdditionalImages", opt => opt.MapFrom(src => src.Images != null 
                ? src.Images.Select(i => i.ImagePath).ToList() 
                : new List<string>()));
        
        // Все остальные поля (Id, Model, Price, Year, Color, Horsepower, EngineVolume, Acceleration0to100, MainImage)
        // AutoMapper сопоставит автоматически, так как их имена в Car и CarDetailDto СОВПАДАЮТ.

        // 2. МАППИНГ ДЛЯ СПИСКА (CarListItemDto - RECORD)
        CreateMap<Car, CarListItemDto>()
            .ForCtorParam("BrandName", opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : "No Brand"))
            .ForCtorParam("CarClass", opt => opt.MapFrom(src => src.Class.ToString()));
            
        // 3. ЕСЛИ ТЕБЕ НУЖЕН МАППИНГ ОБРАТНО (Для создания/обновления, если используешь маппер там)
        // Обычно для CarCreateRequest -> Car маппинг пишется так:
        // CreateMap<CarCreateRequest, Car>().ForMember(x => x.MainImage, opt => opt.Ignore());
    }
}