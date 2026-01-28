// CarX.Application.DTOs/Car/CarListItemDto.cs

using System.ComponentModel.DataAnnotations;
using CarX.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CarXWebApi.Application.Dtos;

public record CarListItemDto(
    long Id,
    string Model,
    string BrandName,
    decimal Price,
    int Year,
    string? CarImage,
    string CarClass // Отдаем строкой: "Budget", "Sport" и т.д.
);

// CarX.Application.DTOs/Car/CarDetailDto.cs
public record CarDetailDto(
    long Id,
    string Model,
    string BrandName,
    string BrandCountry,
    decimal Price,
    int Year,
    string? CarImage,
    string CarClass,
    string? BrandImage // Логотип бренда в деталях машины
);