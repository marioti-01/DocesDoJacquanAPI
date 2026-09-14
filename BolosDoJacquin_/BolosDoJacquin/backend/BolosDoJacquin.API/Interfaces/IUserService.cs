using BolosDoJacquin.API.DTOs;

namespace BolosDoJacquin.API.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> List();

    Task<UserStatusResult> Status(
        Guid id,
        bool active
    );
}

public enum UserStatusResult
{
    Success,
    NotFound
}