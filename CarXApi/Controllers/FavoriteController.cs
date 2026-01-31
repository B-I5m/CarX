using System.Security.Claims;
using CarX.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarX.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Tags("Favorites")] 
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    public FavoritesController(IFavoriteService favoriteService) => _favoriteService = favoriteService;

    [HttpPost("car/{carId}")]
    public async Task<IActionResult> ToggleCar(long carId)
    {
        var result = await _favoriteService.ToggleFavoriteAsync(GetUserId(), carId, null);
        return Ok(new { Message = result });
    }

    [HttpPost("rent-car/{rentCarId}")]
    public async Task<IActionResult> ToggleRentCar(long rentCarId)
    {
        var result = await _favoriteService.ToggleFavoriteAsync(GetUserId(), null, rentCarId);
        return Ok(new { Message = result });
    }

    // --- ВОТ ОНИ, ТВОИ DELETE КНОПКИ ---

    [HttpDelete("car/{carId}")]
    public async Task<IActionResult> DeleteCarFavorite(long carId)
    {
        var deleted = await _favoriteService.RemoveCarFavoriteAsync(GetUserId(), carId);
        return deleted ? NoContent() : NotFound(new { Message = "Машина не найдена в избранном" });
    }

    [HttpDelete("rent-car/{rentCarId}")]
    public async Task<IActionResult> DeleteRentCarFavorite(long rentCarId)
    {
        var deleted = await _favoriteService.RemoveRentCarFavoriteAsync(GetUserId(), rentCarId);
        return deleted ? NoContent() : NotFound(new { Message = "Рент-машина не найдена в избранном" });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyFavorites() => Ok(await _favoriteService.GetUserFavoritesAsync(GetUserId()));

    private long GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("id");
        return long.Parse(claim!.Value);
    }
}