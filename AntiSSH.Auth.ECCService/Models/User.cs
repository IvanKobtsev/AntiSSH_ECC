using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AntiSSH.Auth.ECC.Models;

public class User : IdentityUser<Guid>
{
    [MaxLength(50)] public string FirstName { get; set; } = string.Empty;
    [MaxLength(50)] public string LastName { get; set; } = string.Empty;
    [MaxLength(50)] public string Patronymic { get; set; } = string.Empty;
}