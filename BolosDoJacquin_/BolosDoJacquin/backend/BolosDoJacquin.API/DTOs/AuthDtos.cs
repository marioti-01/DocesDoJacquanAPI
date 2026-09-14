using System.ComponentModel.DataAnnotations;
namespace BolosDoJacquin.API.DTOs;
public record RegisterDto([Required, MaxLength(120)] string Name, [Required, EmailAddress] string Email, [Required, MinLength(6)] string Password);
public record LoginDto([Required, EmailAddress] string Email, [Required] string Password);
public record AuthResponseDto(string Token, Guid Id, string Name, string Email, string Role);
public record UserDto(Guid Id, string Name, string Email, string Role, string Status, DateTime CreatedAt);
