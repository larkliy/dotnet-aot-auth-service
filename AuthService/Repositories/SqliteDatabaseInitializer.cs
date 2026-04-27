using Dapper;
using System.Data;

namespace AuthService.Repositories;

public class SqliteDatabaseInitializer(IDbConnection db) : IDatabaseInitializer
{
    public async Task InitializeAsync()
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Email TEXT NOT NULL,
            PasswordHash TEXT NOT NULL,
            Role TEXT NOT NULL DEFAULT 'User', -- ДОБАВИТЬ ЭТУ СТРОКУ
            CreatedAt TEXT NOT NULL,
            RefreshToken TEXT NULL,
            RefreshTokenExpiryTime TEXT NULL
        );

            CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_Email ON Users(Email);
        ";

        await db.ExecuteAsync(sql);

        var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
        if (count == 0)
        {
            await db.ExecuteAsync(
                @"INSERT INTO Users (Email, PasswordHash, CreatedAt)
                  VALUES (@Email, @PasswordHash, @CreatedAt)",
                new
                {
                    Email = "firstUser@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("5555551234"),
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
