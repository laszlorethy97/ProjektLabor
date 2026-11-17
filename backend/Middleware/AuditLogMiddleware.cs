using System.Diagnostics;
using System.Security.Claims;
using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Middleware;

public class AuditLogMiddleware
{
    private static readonly Action<ILogger, string, Exception?> AuditEventPersistenceFailed =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, nameof(AuditEventPersistenceFailed)),
            "Could not persist HTTP audit event {CorrelationId}.");

    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLogMiddleware> _logger;

    public AuditLogMiddleware(
        RequestDelegate next,
        ILogger<AuditLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        catch
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            throw;
        }
        finally
        {
            stopwatch.Stop();
            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? actorUserId = int.TryParse(userIdClaim, out var parsedUserId)
                ? parsedUserId
                : null;
            var resource = context.Request.Path.Value ?? string.Empty;
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var method = context.Request.Method;

            var auditEvent = new AuditLog
            {
                EventType = AuditLogEventTypes.HttpRequest,
                ActorUserId = actorUserId,
                ActorEmail = Truncate(context.User.FindFirstValue(ClaimTypes.Email), 256),
                ActorRoles = Truncate(string.Join(
                    ",",
                    context.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).Distinct()), 256),
                Method = method.Length <= 10 ? method : method[..10],
                Resource = resource.Length <= 512 ? resource : resource[..512],
                CorrelationId = Truncate(context.TraceIdentifier, 64),
                StatusCode = context.Response.StatusCode,
                OccurredAt = startedAt,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = userAgent.Length <= 512 ? userAgent : userAgent[..512],
                DurationMilliseconds = stopwatch.ElapsedMilliseconds
            };

            await using var scope = context.RequestServices.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<KeyManagementDbContext>();
            dbContext.AuditLogs.Add(auditEvent);
            try
            {
                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception exception)
            {
                AuditEventPersistenceFailed(_logger, context.TraceIdentifier, exception);
            }
        }
    }

    private static string? Truncate(string? value, int maximumLength)
    {
        return value is { Length: > 0 }
            ? value.Length <= maximumLength ? value : value[..maximumLength]
            : null;
    }
}
