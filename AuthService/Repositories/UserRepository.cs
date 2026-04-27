using System.Data;
using AuthService.Models;
using Dapper;

namespace AuthService.Repositories;

public class UserRepository(IDbConnection db) : IUserRepository
{
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var sql = "SELECT EXISTS (SELECT 1 FROM Users WHERE Email = @Email)";
        return await db.ExecuteScalarAsync<bool>(sql, new { Email = email });
    }

    public Task CreateUserAsync(string email, string passwordHash, DateTime createdAt)
    {
        var sql = @"INSERT INTO Users (Email, PasswordHash, CreatedAt)
                    VALUES (@Email, @PasswordHash, @CreatedAt)";
        return db.ExecuteAsync(sql, new { Email = email, PasswordHash = passwordHash, CreatedAt = createdAt });
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var sql = "SELECT Id, Email, PasswordHash, Role, CreatedAt, RefreshToken, RefreshTokenExpiryTime FROM Users WHERE Email = @Email LIMIT 1";
        return db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime)
    {
        var sql = @"UPDATE Users 
                    SET RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @RefreshTokenExpiryTime
                    WHERE Id = @Id";
        return db.ExecuteAsync(sql, new { RefreshToken = refreshToken, RefreshTokenExpiryTime = expiryTime, Id = userId });
    }
}