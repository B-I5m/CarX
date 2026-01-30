using System.ComponentModel.DataAnnotations;

namespace CarX.Application.Dtos;

public class RegisterDto
{
    [Required(ErrorMessage = "Имя пользователя обязательно")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [DataType(DataType.Password)]
    [MinLength(4, ErrorMessage = "Пароль должен быть длиннее 4 символов")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтверждение пароля обязательно")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    // Твое кастомное поле из сущности User
    [Required(ErrorMessage = "Имя (FirstName) обязательно")]
    public string FirstName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    
}