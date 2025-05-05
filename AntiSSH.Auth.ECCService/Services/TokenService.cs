using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AntiSSH.Auth.ECC.Data;
using Microsoft.IdentityModel.Tokens;

namespace AntiSSH.Auth.ECC.Services;

public class TokenService(
    IConfiguration config,
    CustomEcdsaService ecdsaService,
    ApplicationDbContext dbContext
)
{
    public string GenerateToken(string userId)
    {
        var payloadObj = new
        {
            sub = userId,
            exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
        };
        var payloadJson = JsonSerializer.Serialize(payloadObj);
        var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);

        var (r, s) = ecdsaService.SignMessage(new BigInteger(GetPrivateKey()), payloadBytes);

        var token =
            $"{Base64Url(payloadBytes)}.{Base64Url(r.ToByteArray())}.{Base64Url(s.ToByteArray())}";
        return token;
    }

    public string? TryValidateToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            return null;

        try
        {
            var data = Base64UrlDecode(parts[0]);
            var r = Base64UrlDecode(parts[1]);
            var s = Base64UrlDecode(parts[2]);

            var valid = ecdsaService.VerifySignature(
                new BigInteger(GetPrivateKey()),
                data,
                new BigInteger(r),
                new BigInteger(s)
            );

            if (!valid)
                return null;

            var payloadJson = Encoding.UTF8.GetString(data);
            var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);

            if (payload?["exp"].ToString() != null)
            {
                var exp = UnixTimeStampToDateTime(double.Parse(payload["exp"].ToString()!));

                if (exp > DateTime.UtcNow)
                    return payload["sub"].ToString();
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    private static string Base64Url(byte[] input)
    {
        return Convert.ToBase64String(input).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        input = input.Replace("-", "+").Replace("_", "/");
        switch (input.Length % 4)
        {
            case 2:
                input += "==";
                break;
            case 3:
                input += "=";
                break;
        }
        return Convert.FromBase64String(input);
    }

    private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    private byte[] GetPrivateKey()
    {
        var passphrase =
            File.ReadAllText("./SecretVault/passphrase.txt")
            ?? throw new InvalidOperationException("Passphrase not found");

        var encryptedKey =
            dbContext.EncryptedKeys.FirstOrDefault(ek => ek.Name == "ECCPrivateKey")
            ?? throw new InvalidOperationException("Encrypted key not found in the database");

        return PrivateKeyService.DecryptPrivateKey(
            encryptedKey.Key,
            passphrase,
            encryptedKey.Salt,
            encryptedKey.Iv
        );
    }
}
