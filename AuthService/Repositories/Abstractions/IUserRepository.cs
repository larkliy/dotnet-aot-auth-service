using AuthService.Models;

namespace AuthService.Repositories.Abstractions;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateUserAsync(string email, string passwordHash, string role, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(int id, string email, string role, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(int id, CancellationToken cancellationToken = default);
}