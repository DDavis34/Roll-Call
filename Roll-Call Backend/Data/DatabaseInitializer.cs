using Microsoft.EntityFrameworkCore;

namespace RollCallBackend.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext dbContext, CancellationToken ct = default)
    {
        await dbContext.Database.EnsureCreatedAsync(ct);

        await EnsureSavedCharacterOwnershipColumnAsync(dbContext, ct);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS "IX_SavedCharacters_UserId_UpdatedUtc"
            ON "SavedCharacters" ("UserId", "UpdatedUtc")
            """,
            ct);
    }

    private static async Task EnsureSavedCharacterOwnershipColumnAsync(AppDbContext dbContext, CancellationToken ct)
    {
        await using var connection = dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(ct);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM pragma_table_info('SavedCharacters')
            WHERE name = 'UserId'
            """;

        var result = await command.ExecuteScalarAsync(ct);
        var hasUserIdColumn = Convert.ToInt32(result) > 0;

        if (!hasUserIdColumn)
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                ALTER TABLE "SavedCharacters"
                ADD COLUMN "UserId" TEXT NOT NULL DEFAULT ''
                """,
                ct);
        }
    }
}
