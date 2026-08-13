using AuthService.Repositories.Abstractions;
using Dapper;
using System.Data;

namespace AuthService.Repositories;

public class SqliteDatabaseInitializer(IDbConnection db) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        /* 
         * IMPORTANT: NATIVE AOT COMPATIBILITY NOTE
         * 
         * We are currently bypassing the use of 'CommandDefinition' and 'CancellationToken'.
         * 
         * REASON: 
         * The Dapper.AOT Source Generator (v1.0.33) has a bug when analyzing 'CommandDefinition' 
         * in certain environments (especially .NET 9/10 preview). It produces invalid 
         * generated C# code: "Error CS0103: The name 'cmd' does not exist in the current context".
         * 
         * WORKAROUND:
         * To ensure the Source Generator correctly intercepts calls and avoids falling back to 
         * Reflection.Emit (which is unsupported in Native AOT), we use the simplest method 
         * overloads: ExecuteAsync(sql) and ExecuteScalarAsync<T>(sql, param).
         * 
         * TODO: Restore CancellationToken support once Dapper.AOT generator is fixed to 
         * correctly handle CommandDefinition scope in its interceptor logic.
         */

        var sql = @"
            CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Email TEXT NOT NULL,
            PasswordHash TEXT NOT NULL,
            Role TEXT NOT NULL DEFAULT 'User',
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
            var insertSql = @"INSERT INTO Users (Email, PasswordHash, CreatedAt)
                              VALUES (@Email, @PasswordHash, @CreatedAt)";

            var parameters = new
            {
                Email = "firstUser@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("5555551234"),
                CreatedAt = DateTime.UtcNow
            };

            await db.ExecuteAsync(insertSql, parameters);
        }
    }
}