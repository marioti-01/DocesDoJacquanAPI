using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BolosDoJacquin.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var response = await _authService.Register(dto);

        if (response is null)
        {
            return Conflict(new
            {
                message = "E-mail já cadastrado."
            });
        }

        return Created("api/auth/me", response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "E-mail ou senha inválidos."
            });
        }

        if (result.UserInactive)
        {
            return StatusCode(403, new
            {
                message = "Usuário desativado."
            });
        }

        return Ok(result.Response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(claim))
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(claim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _authService.Me(userId);

        if (user is null)
        {
            return NotFound(new
            {
                message = "Usuário não encontrado."
            });
        }

        return Ok(user);
    }
}