namespace CarX.Application.Dtos;

public record RentCreateRequest(
    long RentCarId, // Было CarId, теперь RentCarId
    long UserId, 
    DateTime FromDate, 
    DateTime ToDate
);

public record RentCarCreateRequest(
    string Model,
    long BrandId,
    decimal PricePerDay,
    decimal Deposit,
    int Year,
    string Transmission
);