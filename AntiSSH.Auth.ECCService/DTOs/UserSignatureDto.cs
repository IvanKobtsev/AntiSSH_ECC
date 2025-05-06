namespace AntiSSH.Auth.ECC.DTOs;

public class UserSignatureDto
{
    public string Username { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public required EccSignatureDto Signature { get; set; }
}
