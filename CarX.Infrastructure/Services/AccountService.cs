using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using CarX.Application.Dtos;
using CarX.Application.Interfaces;
using CarX.Domain.Entities;
using CarX.Domain.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CarX.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<long>> _roleManager;
    private readonly IConfiguration _configuration;

    public AccountService(
        UserManager<User> userManager, 
        RoleManager<IdentityRole<long>> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    
    public async Task<Response<RegisterDto>> Register(RegisterDto model)
    {
        var user = new User()
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            FirstName = model.FirstName // Исправил: берем FirstName из модели
        };

        var result = await _userManager.CreateAsync(user, model.Password);
    
        if (result.Succeeded)
        {
            // ЖЕСТКО: При регистрации через API всегда даем роль "User"
            string defaultRole = "User";
        
            if (!await _roleManager.RoleExistsAsync(defaultRole))
                await _roleManager.CreateAsync(new IdentityRole<long>(defaultRole));

            await _userManager.AddToRoleAsync(user, defaultRole);
        
            return new Response<RegisterDto>(model);
        }

        var errors = string.Join(", ", result.Errors.Select(x => x.Description));
        return new Response<RegisterDto>(HttpStatusCode.BadRequest, errors);
    }

    public async Task<Response<string>> Login(LoginDto login)
    {
        var user = await _userManager.FindByNameAsync(login.Username);
        if (user != null)
        {
            var checkPassword = await _userManager.CheckPasswordAsync(user, login.Password);
            if (checkPassword)
            {
                var token = await GenerateJwtToken(user);
                return new Response<string>(token);
            }
        }
        return new Response<string>(HttpStatusCode.BadRequest, "Неверный логин или пароль");
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // Id теперь long
        };
        
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Response<string>> AddOrRemoveUserFromRole(UserRoleDto userRole, bool delete = false)
    {
        // Превращаем long в string, чтобы FindByIdAsync не ругался
        var user = await _userManager.FindByIdAsync(userRole.UserId.ToString());
        var role = await _roleManager.FindByIdAsync(userRole.RoleId.ToString());
    
        if (user == null) return new Response<string>(HttpStatusCode.NotFound, "Пользователь не найден");
        if (role == null) return new Response<string>(HttpStatusCode.NotFound, "Роль не найдена");

        if (delete)
        {
            await _userManager.RemoveFromRoleAsync(user, role.Name!);
            return new Response<string>("Роль удалена");
        }

        // Проверяем, нет ли уже такой роли у юзера
        var userInRole = await _userManager.IsInRoleAsync(user, role.Name!);
        if (userInRole) return new Response<string>(HttpStatusCode.BadRequest, "У пользователя уже есть эта роль");

        await _userManager.AddToRoleAsync(user, role.Name!);
        return new Response<string>("Роль успешно добавлена");
    }
    public async Task<Response<List<User>>> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return new Response<List<User>>(users);
    }
}