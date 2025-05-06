using System.ComponentModel.DataAnnotations;

namespace AntiSSH.Auth.ECC.DTOs;

public class UpdateProfileDto
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
