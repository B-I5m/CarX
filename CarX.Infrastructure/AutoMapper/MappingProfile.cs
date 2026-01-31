using AutoMapper;
using CarX.Application.Dtos;
using CarX.Domain.Entities;
using System.Linq;
using System.Collections.Generic; // Обязательно добавь это!

namespace CarX.Infrastructure.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 1. МАППИНГ ДЛЯ ДЕТАЛЬНОЙ ИНФОРМАЦИИ
        CreateMap<Car, CarDetailDto>()
            .ForCtorParam("BrandName", opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : "No Brand"))
            // Добавляем безопасное приведение к строке для Enum
            .ForCtorParam("Transmission", opt => opt.MapFrom(src => src.Transmission.ToString()))
            .ForCtorParam("BodyType", opt => opt.MapFrom(src => src.BodyType.ToString()))
            // Безопасный выбор картинок
            .ForCtorParam("AdditionalImages", opt => opt.MapFrom(src => 
                (src.Images != null && src.Images.Any()) 
                    ? src.Images.Select(i => i.ImagePath).ToList() 
                    : new List<string>()));
        
        // 2. МАППИНГ ДЛЯ СПИСКА
        CreateMap<Car, CarListItemDto>()
            .ForCtorParam("BrandName", opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : "No Brand"))
            // Исправляем возможную ошибку с Enum
            .ForCtorParam("CarClass", opt => opt.MapFrom(src => src.Class.ToString()));

        // --- НОВОЕ: Маппинг для Избранного (если создашь FavoriteDto) ---
        // CreateMap<Favorite, FavoriteDto>(); 
    }
}