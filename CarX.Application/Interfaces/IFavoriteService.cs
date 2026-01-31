namespace CarX.Infrastructure.Services;

public interface IFavoriteService
{
    Task<string> ToggleFavoriteAsync(long userId, long? carId, long? rentCarId);
    Task<IEnumerable<object>> GetUserFavoritesAsync(long userId);
    
    // Новые методы для явного удаления
    Task<bool> RemoveCarFavoriteAsync(long userId, long carId);
    Task<bool> RemoveRentCarFavoriteAsync(long userId, long rentCarId);
}