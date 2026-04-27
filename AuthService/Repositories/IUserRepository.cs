using AuthService.Models;

namespace AuthService.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task CreateUserAsync(string email, string passwordHash, DateTime createdAt);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime);
}