using BolosDoJacquin.API.Data;
using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using BolosDoJacquin.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BolosDoJacquin.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;

    public AuthService(AppDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    public async Task<AuthResponseDto?> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var emailExiste = await _db.Users
            .AnyAsync(x => x.Email == email);

        if (emailExiste)
            return null;

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Cliente,
            Status = RecordStatus.Ativo
        };

        _db.Users.Add(user);

        await _db.SaveChangesAsync();

        var token = _tokens.Generate(user);

        return new AuthResponseDto(
            token,
            user.Id,
            user.Name,
            user.Email,
            user.Role.ToString()
        );
    }

    public async Task<LoginResult?> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
            return null;

        var senhaValida = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            user.PasswordHash
        );

        if (!senhaValida)
            return null;

        if (user.Status == RecordStatus.Inativo)
            return new LoginResult(null, true);

        var token = _tokens.Generate(user);

        var response = new AuthResponseDto(
            token,
            user.Id,
            user.Name,
            user.Email,
            user.Role.ToString()
        );

        return new LoginResult(response, false);
    }

    public async Task<UserDto?> Me(Guid id)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
            return null;

        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.Role.ToString(),
            user.Status.ToString(),
            user.CreatedAt
        );
    }
}