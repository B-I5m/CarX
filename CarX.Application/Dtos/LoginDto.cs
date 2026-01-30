using System.ComponentModel.DataAnnotations;

namespace CarX.Application.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "Введите имя пользователя")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}