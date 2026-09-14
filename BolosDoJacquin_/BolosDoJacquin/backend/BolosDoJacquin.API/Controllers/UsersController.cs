using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolosDoJacquin.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Administrador")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> List()
    {
        var users = await _userService.List();

        return Ok(users);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> Status(
        Guid id,
        [FromBody] bool active)
    {
        var result = await _userService.Status(
            id,
            active);

        if (result == UserStatusResult.NotFound)
        {
            return NotFound(new
            {
                message = "Usuário não encontrado."
            });
        }

        return NoContent();
    }
}