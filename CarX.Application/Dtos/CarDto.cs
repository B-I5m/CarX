namespace CarX.Application.Dtos;

// CarX.Application.DTOs/Order/OrderCreateRequest.cs


// CarX.Application.DTOs/Rent/RentCreateRequest.cs

// CarX.Application.DTOs/Car/CarDetailDto.cs (Обновленный)
public record CarDetailDto(
    long Id,
    string Model,
    string BrandName,
    decimal Price,
    int Year,
    string Color,
    int Horsepower,
    double EngineVolume,
    double Acceleration0to100,
    string Transmission, // Будет строкой после маппинга
    string BodyType,     // Будет строкой после маппинга
    string? MainImage, 
    List<string> AdditionalImages
);
// CarX.Application.DTOs/Car/CarListItemDto.cs

public record CarListItemDto(
    long Id,
    string Model,
    string BrandName,
    decimal Price,
    int Year,
    string? MainImage, // Главное фото
    string CarClass
);