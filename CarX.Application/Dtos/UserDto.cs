namespace CarX.Application.DTOs;

// CarX.Application.DTOs/User/UserDto.cs
public class UserDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } // "Admin" или "User"
}