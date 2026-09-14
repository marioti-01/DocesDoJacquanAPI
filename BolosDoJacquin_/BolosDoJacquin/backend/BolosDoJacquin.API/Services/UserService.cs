using BolosDoJacquin.API.Data;
using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using BolosDoJacquin.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BolosDoJacquin.API.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> List()
    {
        return await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .Select(u => new UserDto(
                u.Id,
                u.Name,
                u.Email,
                u.Role.ToString(),
                u.Status.ToString(),
                u.CreatedAt
            ))
            .ToListAsync();
    }

    public async Task<UserStatusResult> Status(
        Guid id,
        bool active)
    {
        var user = await _db.Users
            .FindAsync(id);

        if (user is null)
            return UserStatusResult.NotFound;

        user.Status = active
            ? RecordStatus.Ativo
            : RecordStatus.Inativo;

        await _db.SaveChangesAsync();

        return UserStatusResult.Success;
    }
}