using System.ComponentModel.DataAnnotations;

namespace AntiSSH.Auth.ECC.DTOs;

public class UpdateProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Patronymic { get; set; } = string.Empty;

    [EmailAddress] public string Email { get; set; } = string.Empty;

    [Phone]
    [MinLength(11)]
    [MaxLength(11)]
    public string PhoneNumber { get; set; } = string.Empty;
}