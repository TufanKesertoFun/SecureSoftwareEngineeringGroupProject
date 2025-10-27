using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SecureSoftwareGroupProject.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
            logger.LogDebug("Database schema verified.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to ensure the application database is ready.");
            throw;
        }
    }
}
