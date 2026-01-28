// CarX.Application.DTOs/Brand/BrandDto.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public record BrandDto(
    long Id, 
    string Name, 
    string Country, 
    string? BrandImage // Путь к картинке
);

// CarX.Application.DTOs/Brand/BrandCreateRequest.cs
public class BrandCreateRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Country { get; set; } = string.Empty;
    public IFormFile? ImageFile { get; set; } // Для кнопки "Выбрать файл"
}