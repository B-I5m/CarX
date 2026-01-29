// CarX.Application.DTOs/Order/OrderCreateRequest.cs
namespace CarX.Application.Dtos;

public record OrderCreateRequest(
    long CarId, 
    long UserId
);
