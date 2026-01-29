// CarX.Application.DTOs/Rent/RentCreateRequest.cs
namespace CarX.Application.Dtos;

public record RentCreateRequest(
    long CarId, 
    long UserId, 
    DateTime FromDate, 
    DateTime ToDate
);