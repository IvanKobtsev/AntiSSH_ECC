using System.Security.Cryptography;
using AntiSSH.Auth.ECC.Data;
using AntiSSH.Auth.ECC.DTOs;
using AntiSSH.Auth.ECC.Enums;
using AntiSSH.Auth.ECC.Mappers;
using AntiSSH.Auth.ECC.Models;
using AntiSSH.Auth.ECC.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AntiSSH.Auth.ECC.Services;

public class UserService(ApplicationDbContext dbContext, TokenService tokenService)
{
    public async Task<Result<ChallengeDto>> GetNonce(LoginDto loginDto)
    {
        var foundUser = await dbContext
            .Users.Include(u => u.EncryptedKey)
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (foundUser == null)
            return new Result<ChallengeDto>
            {
                Message = "Invalid credentials",
                Code = HttpCode.BadRequest,
            };

        if (foundUser.EncryptedKey?.PublicKey == null)
        {
            throw new InvalidOperationException("Invalid data in database");
        }

        var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        foundUser.EncryptedKey.LastNonce = nonce;
        await dbContext.SaveChangesAsync();

        var challenge = new ChallengeDto
        {
            Nonce = nonce,
            EncryptedKey = new EncryptedKeyDto
            {
                EncryptedPrivateKey = TokenService.Base64Url(foundUser.EncryptedKey.PrivateKey),
                Iv = TokenService.Base64Url(foundUser.EncryptedKey.Iv),
                Salt = TokenService.Base64Url(foundUser.EncryptedKey.Salt),
                PublicKey = TokenService.Base64Url(foundUser.EncryptedKey.PublicKey),
            },
        };

        return new Result<ChallengeDto> { Data = challenge };
    }

    public async Task<Result<ProfileDto>> GetProfileDto(Guid userId)
    {
        var foundUser = await dbContext.Users.FindAsync(userId);

        return foundUser == null
            ? new Result<ProfileDto> { Message = "User not found", Code = HttpCode.NotFound }
            : new Result<ProfileDto> { Data = foundUser.ToDto() };
    }

    public async Task<Result<TokenDto>> Login(UserSignatureDto signatureDto)
    {
        var foundUser = await dbContext
            .Users.Include(u => u.EncryptedKey)
            .FirstOrDefaultAsync(u => u.Username == signatureDto.Username);

        if (foundUser == null)
            return new Result<TokenDto>
            {
                Message = "Invalid credentials",
                Code = HttpCode.BadRequest,
            };

        if (foundUser.EncryptedKey?.PublicKey == null)
        {
            throw new InvalidOperationException("Invalid data in database");
        }

        var isValid = SignatureVerifier.VerifySignature(
            foundUser.EncryptedKey.PublicKey,
            TokenService.Base64UrlDecode(signatureDto.Nonce),
            signatureDto.Signature
        );

        if (!isValid || foundUser.EncryptedKey.LastNonce != signatureDto.Nonce)
            return new Result<TokenDto>
            {
                Message = "Invalid signature",
                Code = HttpCode.BadRequest,
            };

        foundUser.EncryptedKey.LastNonce = null;
        await dbContext.SaveChangesAsync();

        return new Result<TokenDto>
        {
            Data = new TokenDto()
            {
                AccessToken = tokenService.GenerateToken(foundUser.Id.ToString()),
            },
        };
    }

    public async Task<Result> CreateUser(RegisterUserDto registerUserDto)
    {
        var createdUser = new User
        {
            Username = registerUserDto.Username,
            Email = registerUserDto.Email,
            FullName = registerUserDto.FullName,
            Id = Guid.NewGuid(),
        };

        var createdKeyData = new EncryptedKey
        {
            Id = Guid.NewGuid(),
            Name = "UserKey",
            PrivateKey = TokenService.Base64UrlDecode(
                registerUserDto.EncryptedKey.EncryptedPrivateKey
            ),
            PublicKey = TokenService.Base64UrlDecode(registerUserDto.EncryptedKey.PublicKey),
            Salt = TokenService.Base64UrlDecode(registerUserDto.EncryptedKey.Salt),
            Iv = TokenService.Base64UrlDecode(registerUserDto.EncryptedKey.Iv),
        };

        createdUser.EncryptedKey = createdKeyData;

        await dbContext.EncryptedKeys.AddAsync(createdKeyData);
        await dbContext.Users.AddAsync(createdUser);
        await dbContext.SaveChangesAsync();

        // var token = tokenService.GenerateToken(createdUser.Id.ToString());

        return new Result();
    }

    public async Task<Result> UpdateProfile(Guid userId, UpdateProfileDto updateProfileDto)
    {
        var user = await dbContext.Users.FindAsync(userId);

        if (user == null)
            return new Result { Code = HttpCode.NotFound, Message = "User not found" };

        user.Username = updateProfileDto.Username;
        user.Email = updateProfileDto.Email;
        user.FullName = updateProfileDto.FullName;

        await dbContext.SaveChangesAsync();

        return new Result();
    }

    public async Task<Result> DeleteProfile(Guid userId)
    {
        var user = await dbContext.Users.FindAsync(userId);

        if (user == null)
            return new Result { Code = HttpCode.NotFound, Message = "User not found" };

        var result = dbContext.Users.Remove(user);

        return new Result();
    }
}
