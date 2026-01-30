using System.Net;
using CarX.Application.Dtos;
using CarX.Application.Interfaces;
using CarX.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarXWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("Register")]
    [AllowAnonymous] // Доступно всем
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new Response<RegisterDto>(HttpStatusCode.BadRequest, errors));
        }

        var response = await _accountService.Register(registerDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Login")]
    [AllowAnonymous] // Доступно всем
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response<string>(HttpStatusCode.BadRequest, "Заполните все поля"));
        }

        var response = await _accountService.Login(loginDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("AddUserToRole")]
    [Authorize(Roles = "Admin")] // Только для админов
    public async Task<IActionResult> AddUserToRole([FromBody] UserRoleDto userRoleDto)
    {
        var response = await _accountService.AddOrRemoveUserFromRole(userRoleDto, false);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("DeleteRoleFromUser")]
    [Authorize(Roles = "Admin")] // Только для админов
    public async Task<IActionResult> DeleteRoleFromUser([FromBody] UserRoleDto userRoleDto)
    {
        var response = await _accountService.AddOrRemoveUserFromRole(userRoleDto, true);
        return StatusCode(response.StatusCode, response);
    }
    [HttpGet("GetAllUsers")]
    [Authorize(Roles = "Admin")] // Только для админов
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _accountService.GetAllUsers();
        return StatusCode(response.StatusCode, response);
    }
}