using AuthService.Models;

namespace AuthService.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task CreateUserAsync(string email, string passwordHash, DateTime createdAt);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime);

    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetByIdAsync(int id);
    Task DeleteUserAsync(int id);
    Task UpdateUserAsync(int id, string email, string role);
    Task AdminCreateUserAsync(string email, string passwordHash, string role, DateTime createdAt);
}