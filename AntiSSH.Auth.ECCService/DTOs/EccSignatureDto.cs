namespace AntiSSH.Auth.ECC.DTOs;

public class EccSignatureDto
{
    public required string R { get; set; }
    public required string S { get; set; }
}
