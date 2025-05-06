using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AntiSSH.Auth.ECC.DTOs;

public class RegisterUserDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [MaxLength(100)]
    [Required]
    public string FullName { get; set; } = string.Empty;

    public required EncryptedKeyDto EncryptedKey { get; set; }
}
