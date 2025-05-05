using System.Security.Claims;
using AntiSSH.Auth.ECC.DTOs;
using AntiSSH.Auth.ECC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiSSH.Auth.ECC.Controllers;

[ApiController]
[Route("/api/v1/users/me")]
public class UserController(UserService userService) : ControllerBase
{
    [Authorize(AuthenticationSchemes = "CustomScheme")]
    [HttpGet("")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return userId == null
            ? Unauthorized()
            : (await userService.GetProfileDto(Guid.Parse(userId))).GetActionResult();
    }

    [Authorize(AuthenticationSchemes = "CustomScheme")]
    [HttpPut("")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return userId == null
            ? Unauthorized()
            : (await userService.UpdateProfile(Guid.Parse(userId), profileDto)).GetActionResult();
    }
}
