using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AntiSSH.Auth.ECC.Models;

public class User
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public Guid EncryptedKeyId { get; set; }
    public EncryptedKey EncryptedKey { get; set; }
}
