namespace AntiSSH.Auth.ECC.DTOs;

public class ChallengeDto
{
    public required string Nonce { get; set; }
    public required EncryptedKeyDto EncryptedKey { get; set; }
}
