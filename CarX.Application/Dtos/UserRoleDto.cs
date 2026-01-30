using System.ComponentModel.DataAnnotations;

namespace CarX.Application.Dtos;

public class UserRoleDto
{
    [Required(ErrorMessage = "ID пользователя обязателен")]
    public long UserId { get; set; }

    [Required(ErrorMessage = "ID роли обязателен")]
    public long RoleId { get; set; }
}