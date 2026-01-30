// CarXWebApi/Controllers/RentsController.cs

using CarX.Application.Dtos;
using CarX.Application.Interfaces;
using CarX.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RentsController : ControllerBase
{
    private readonly IRentService _rentService;
    public RentsController(IRentService rentService) => _rentService = rentService;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] RentCreateRequest request) => 
        Ok(await _rentService.CreateRentAsync(request));

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll() => Ok(await _rentService.GetAllRentsAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(long id) => Ok(await _rentService.GetByIdAsync(id));

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] RentStatus status) => 
        await _rentService.UpdateRentStatusAsync(id, status) ? Ok() : NotFound();

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id) => 
        await _rentService.DeleteRentAsync(id) ? Ok() : NotFound();
}