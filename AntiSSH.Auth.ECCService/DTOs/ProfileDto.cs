namespace AntiSSH.Auth.ECC.DTOs;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public required List<string> Roles { get; set; }
}