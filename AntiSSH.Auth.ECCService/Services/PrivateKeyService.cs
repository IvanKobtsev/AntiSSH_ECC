using System.Security.Cryptography;

namespace AntiSSH.Auth.ECC.Services;

public static class PrivateKeyService
{
    public static byte[] EncryptPrivateKey(
        byte[] privateKey,
        string passphrase,
        out byte[] salt,
        out byte[] iv
    )
    {
        salt = RandomNumberGenerator.GetBytes(16);
        iv = RandomNumberGenerator.GetBytes(16);

        using var aes = Aes.Create();
        aes.Key = DeriveKeyFromPassphrase(passphrase, salt);
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(privateKey, 0, privateKey.Length);
    }

    public static byte[] DecryptPrivateKey(
        byte[] encryptedKey,
        string passphrase,
        byte[] salt,
        byte[] iv
    )
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKeyFromPassphrase(passphrase, salt);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(encryptedKey, 0, encryptedKey.Length);
    }

    private static byte[] DeriveKeyFromPassphrase(string passphrase, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            passphrase,
            salt,
            100_000,
            HashAlgorithmName.SHA256
        );
        return pbkdf2.GetBytes(32); // AES-256
    }
}
