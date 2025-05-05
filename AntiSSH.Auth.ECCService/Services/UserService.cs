using System.Security.Cryptography;
using AntiSSH.Auth.ECC.Data;
using AntiSSH.Auth.ECC.DTOs;
using AntiSSH.Auth.ECC.Enums;
using AntiSSH.Auth.ECC.Mappers;
using AntiSSH.Auth.ECC.Models;
using AntiSSH.Auth.ECC.Utilities;
using Microsoft.AspNetCore.Identity;

namespace AntiSSH.Auth.ECC.Services;

public class UserService(
    UserManager<User> userManager,
    ApplicationDbContext dbContext,
    TokenService tokenService
)
{
    public async Task<Result<ProfileDto>> GetProfileDto(Guid userId)
    {
        var foundUser = await dbContext.Users.FindAsync(userId);

        if (foundUser == null)
            return new Result<ProfileDto> { Message = "User not found", Code = HttpCode.NotFound };

        var userRoles = (await userManager.GetRolesAsync(foundUser)).ToList();

        return new Result<ProfileDto> { Data = foundUser.ToDto(userRoles) };
    }

    public async Task<Result<TokenDto>> Login(LoginDto loginDto)
    {
        var foundUser = await userManager.FindByEmailAsync(loginDto.Email);

        if (
            foundUser == null
            || !await userManager.CheckPasswordAsync(foundUser, loginDto.Password)
        )
            return new Result<TokenDto>
            {
                Message = "Invalid credentials",
                Code = HttpCode.BadRequest,
            };

        return new Result<TokenDto>
        {
            Data = new TokenDto()
            {
                AccessToken = tokenService.GenerateToken(foundUser.Id.ToString()),
            },
        };
    }

    public async Task<Result<TokenDto>> CreateUser(RegisterUserDto registerUserDto)
    {
        var result = await userManager.CreateAsync(
            registerUserDto.ToUser(),
            registerUserDto.Password
        );

        if (!result.Succeeded)
            return new Result<TokenDto>
            {
                Code = HttpCode.BadRequest,
                Message = result.Errors.First().Description,
            };

        var createdUser = await userManager.FindByEmailAsync(registerUserDto.Email);

        if (createdUser == null)
            return new Result<TokenDto>
            {
                Code = HttpCode.BadRequest,
                Message = "Something went wrong while creating user",
            };

        var token = tokenService.GenerateToken(createdUser.Id.ToString());

        return new Result<TokenDto> { Data = new TokenDto() { AccessToken = token } };
    }

    public async Task<Result> UpdateProfile(Guid userId, UpdateProfileDto updateProfileDto)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return new Result { Code = HttpCode.NotFound, Message = "User not found" };

        user.UserName = updateProfileDto.Email;
        user.Email = updateProfileDto.Email;
        user.FirstName = updateProfileDto.FirstName;
        user.LastName = updateProfileDto.LastName;
        user.Patronymic = updateProfileDto.Patronymic;
        user.PhoneNumber = updateProfileDto.PhoneNumber;

        var result = await userManager.UpdateAsync(user);

        return !result.Succeeded
            ? new Result { Code = HttpCode.BadRequest, Message = result.Errors.First().Description }
            : new Result();
    }

    public async Task<Result> DeleteProfile(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return new Result { Code = HttpCode.NotFound, Message = "User not found" };

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded
            ? new Result()
            : new Result
            {
                Code = HttpCode.BadRequest,
                Message = result.Errors.First().Description,
            };
    }
}
