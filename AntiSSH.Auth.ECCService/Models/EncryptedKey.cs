using System.ComponentModel.DataAnnotations;

namespace AntiSSH.Auth.ECC.Models;

public class EncryptedKey
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }
    public required byte[] Key { get; set; }
    public required byte[] Salt { get; set; }
    public required byte[] Iv { get; set; }
}
