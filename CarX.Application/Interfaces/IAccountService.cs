using CarX.Application.Dtos;
using CarX.Domain.Entities;
using CarX.Domain.Response;

namespace CarX.Application.Interfaces; // Проверь свой namespace

public interface IAccountService
{
    // Регистрация
    Task<Response<RegisterDto>> Register(RegisterDto model);
    
    // Логин
    Task<Response<string>> Login(LoginDto login);
    
    // Управление ролями (добавить/удалить роль юзеру)
    Task<Response<string>> AddOrRemoveUserFromRole(UserRoleDto userRole, bool delete = false);
    Task<Response<List<User>>> GetAllUsers();
}