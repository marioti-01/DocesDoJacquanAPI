using BolosDoJacquin.API.DTOs;

namespace BolosDoJacquin.API.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> Register(RegisterDto dto);

    Task<LoginResult?> Login(LoginDto dto);

    Task<UserDto?> Me(Guid id);
}

public record LoginResult(
    AuthResponseDto? Response,
    bool UserInactive
);