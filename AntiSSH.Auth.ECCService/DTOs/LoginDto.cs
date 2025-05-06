using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AntiSSH.Auth.ECC.DTOs;

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
}
