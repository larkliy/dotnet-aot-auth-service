using System.Data;
using AuthService.Models;
using AuthService.Repositories.Abstractions;
using Dapper;

namespace AuthService.Repositories;

public class UserRepository(IDbConnection db) : IAuthRepository, IUserRepository
{
    /* 
     * IMPORTANT: NATIVE AOT COMPATIBILITY NOTE
     * 
     * In this repository, we have intentionally removed the use of 'CommandDefinition' 
     * to pass 'CancellationToken'. 
     * 
     * ISSUE:
     * The Dapper.AOT Source Generator (v1.0.33) currently has a bug where it generates 
     * invalid C# code (Error CS0103: The name 'cmd' does not exist in the current context) 
     * when it encounters 'new CommandDefinition(...)' in .NET 9/10 environments.
     * 
     * WORKAROUND:
     * To ensure the Source Generator can successfully intercept these calls and generate 
     * static metadata for Native AOT, we must use the simple method overloads 
     * (e.g., ExecuteAsync(sql, params)). This prevents the application from falling 
     * back to Reflection.Emit, which is unsupported in Native AOT.
     * 
     * CancellationToken is currently suppressed via '_ = cancellationToken' until 
     * a stable fix is released for the Dapper.AOT analyzer.
     */

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT Id, Email, PasswordHash, Role, CreatedAt, RefreshToken, RefreshTokenExpiryTime FROM Users WHERE Email = @Email LIMIT 1";
        return await db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public Task UpdateRefreshTokenAsync(
        int userId,
        string refreshToken,
        DateTime expiryTime,
        CancellationToken cancellationToken = default)
    {
        var sql = @"UPDATE Users 
                    SET RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @RefreshTokenExpiryTime
                    WHERE Id = @Id";
        return db.ExecuteAsync(sql, new { RefreshToken = refreshToken, RefreshTokenExpiryTime = expiryTime, Id = userId });
    }

    public Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var sql = "SELECT Id, Email, Role, CreatedAt FROM Users";
        return db.QueryAsync<User>(sql);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT Id, Email, Role, CreatedAt FROM Users WHERE Id = @Id LIMIT 1";
        return db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public Task CreateUserAsync(
        string email,
        string passwordHash,
        string role,
        CancellationToken cancellationToken = default)
    {
        var sql = @"INSERT INTO Users (Email, PasswordHash, Role, CreatedAt)
                    VALUES (@Email, @PasswordHash, @Role, @CreatedAt)";
        return db.ExecuteAsync(sql, new { Email = email, PasswordHash = passwordHash, Role = role, CreatedAt = DateTime.UtcNow });
    }

    public Task UpdateUserAsync(
        int id,
        string email,
        string role,
        CancellationToken cancellationToken = default)
    {
        var sql = "UPDATE Users SET Email = @Email, Role = @Role WHERE Id = @Id";
        return db.ExecuteAsync(sql, new { Email = email, Role = role, Id = id });
    }

    public Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM Users WHERE Id = @Id";
        return db.ExecuteAsync(sql, new { Id = id });
    }
}