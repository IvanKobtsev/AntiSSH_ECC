using System.Security.Claims;
using AntiSSH.Auth.ECC.DTOs;
using AntiSSH.Auth.ECC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiSSH.Auth.ECC.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController(UserService userService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        return (await userService.GetNonce(loginDto)).GetActionResult();
    }

    [HttpPost("sign")]
    public async Task<IActionResult> Sign([FromBody] UserSignatureDto userSignature)
    {
        return (await userService.Login(userSignature)).GetActionResult();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        return (await userService.CreateUser(registerDto)).GetActionResult();
    }
}
