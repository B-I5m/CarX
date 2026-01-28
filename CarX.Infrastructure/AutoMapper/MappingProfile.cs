using AutoMapper;
using CarX.Application.DTOs;
using CarX.Domain.Entities;
using CarXWebApi.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace CarX.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ==========================================
        // 1. БРЕНДЫ (Brand)
        // ==========================================
        
        // Мапим Brand -> BrandDto (если BrandDto тоже record, используй логику ниже)
        CreateMap<Brand, BrandDto>();

        CreateMap<BrandCreateRequest, Brand>()
            .ForMember(dest => dest.BrandImage, opt => opt.Ignore())
            .ForMember(dest => dest.Cars, opt => opt.Ignore());

        // ==========================================
        // 2. МАШИНЫ (Car) — ИСПРАВЛЕННЫЙ МАППИНГ ДЛЯ RECORD
        // ==========================================
        
        // Мапим список машин. Используем конструктор record!
        CreateMap<Car, CarListItemDto>()
            .ConstructUsing(src => new CarListItemDto(
                src.Id,
                src.Model,
                src.Brand != null ? src.Brand.Name : "Нет бренда",
                src.Price,
                src.Year,
                src.CarImage,
                src.Class.ToString()
            ));

        // Мапим детали машины. Важно соблюдать порядок полей из твоего record!
        CreateMap<Car, CarDetailDto>()
            .ConstructUsing(src => new CarDetailDto(
                src.Id,
                src.Model,
                src.Brand != null ? src.Brand.Name : "Нет бренда",
                src.Brand != null ? src.Brand.Country : string.Empty,
                src.Price,
                src.Year,
                src.CarImage,
                src.Class.ToString(),
                src.Brand != null ? src.Brand.BrandImage : null
            ));

        // Создание машины (из формы в базу)
        CreateMap<CarCreateRequest, Car>()
            .ForMember(dest => dest.CarImage, opt => opt.Ignore())
            .ForMember(dest => dest.Brand, opt => opt.Ignore());


        // ==========================================
        // 3. ПОЛЬЗОВАТЕЛИ (User)
        // ==========================================
        
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleName, 
                opt => opt.MapFrom(src => src.Role.ToString()));
    }
}