using AuthService.Models;

namespace AuthService.Repositories.Abstractions;

public interface IAuthRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task CreateUserAsync(string email, string passwordHash, string role, CancellationToken cancellationToken = default);
    Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime, CancellationToken cancellationToken = default);
}