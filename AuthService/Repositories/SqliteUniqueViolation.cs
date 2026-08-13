using Microsoft.Data.Sqlite;

namespace AuthService.Repositories;

internal static class SqliteUniqueViolation
{
    public static bool Is(SqliteException ex) =>
        ex.SqliteErrorCode == 19 && ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase);
}