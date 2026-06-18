using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ecommerce.Api.HealthChecks;

public sealed class DbContextHealthCheck<TDbContext>(
    TDbContext dbContext,
    ILogger<DbContextHealthCheck<TDbContext>> logger) : IHealthCheck
    where TDbContext : DbContext
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                return HealthCheckResult.Healthy();
            }

            logger.LogWarning(
                "Database readiness check {HealthCheckName} failed for {DbContextType}.",
                context.Registration.Name,
                typeof(TDbContext).Name);

            return HealthCheckResult.Unhealthy("Database connection failed.");
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Database readiness check {HealthCheckName} failed for {DbContextType}.",
                context.Registration.Name,
                typeof(TDbContext).Name);

            return HealthCheckResult.Unhealthy("Database connection failed.", exception);
        }
    }
}
