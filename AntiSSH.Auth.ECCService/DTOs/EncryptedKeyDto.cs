namespace AntiSSH.Auth.ECC.DTOs;

public class EncryptedKeyDto
{
    public string PublicKey { get; set; } = string.Empty;
    public string EncryptedPrivateKey { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Iv { get; set; } = string.Empty;
}
