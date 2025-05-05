using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AntiSSH.Auth.ECC.DTOs;

public class RegisterUserDto
{
    [EmailAddress] [Required] public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    [PasswordPropertyText]
    [Required]
    public string Password { get; set; } = string.Empty;

    [MaxLength(50)] [Required] public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)] [Required] public string LastName { get; set; } = string.Empty;

    public string? Patronymic { get; set; }

    [Phone]
    [MinLength(11)]
    [MaxLength(11)]
    public string? PhoneNumber { get; set; }
}