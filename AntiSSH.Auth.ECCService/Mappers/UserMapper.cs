using AntiSSH.Auth.ECC.DTOs;
using AntiSSH.Auth.ECC.Models;

namespace AntiSSH.Auth.ECC.Mappers;

public static class UserMapper
{
    public static ProfileDto ToDto(this User user)
    {
        return new ProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Fullname = user.FullName,
        };
    }

    public static User ToUser(this RegisterUserDto userDto)
    {
        return new User
        {
            Username = userDto.Username,
            Email = userDto.Email,
            FullName = userDto.FullName,
        };
    }
}
