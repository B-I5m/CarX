using CarX.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CarX.Domain.Entities;

public class User : IdentityUser<long>
{
    public string FirstName { get; set; }
}