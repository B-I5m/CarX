// CarX.Application.Dtos/Car/CarMatchRequest.cs
using CarX.Domain.Enums;

namespace CarX.Application.Dtos;

public record CarMatchRequest(
    string? Model,        
    string? Color,
    int? MinYear,         
    decimal? MaxPrice,
    int? MinHorsepower,   
    double? MinEngineVolume,     
    double? MaxAcceleration,     
    TransmissionType? Transmission,
    BodyType? BodyType
);