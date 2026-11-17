using System.Globalization;
using System.Security.Claims;
using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class AuditLogPersistenceTest
{
    [Fact]
    public async Task SaveChangesAsyncAuditsDataChangesAndRedactsCredentials()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "7"),
                    new Claim(ClaimTypes.Email, "admin@example.com"),
                    new Claim(ClaimTypes.Role, "admin")
                ], "test"))
            }
        };
        using var context = new KeyManagementDbContext(options, httpContextAccessor);
        var user = new User
        {
            Email = "operator@example.com",
            PasswordHash = "password-hash-secret",
            PinCode = "731942"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var createdEvent = await context.AuditLogs.SingleAsync(entry =>
            entry.EventType == AuditLogEventTypes.DataChange &&
            entry.EntityName == nameof(User) &&
            entry.Method == "CREATE");

        Assert.Contains(user.Id.ToString(CultureInfo.InvariantCulture), createdEvent.EntityId);
        Assert.Contains("[REDACTED]", createdEvent.Details);
        Assert.DoesNotContain(user.PasswordHash, createdEvent.Details);
        Assert.DoesNotContain(user.PinCode, createdEvent.Details);
        Assert.Equal(7, createdEvent.ActorUserId);
        Assert.Equal("admin@example.com", createdEvent.ActorEmail);
        Assert.Equal("admin", createdEvent.ActorRoles);

        user.Email = "renamed@example.com";
        await context.SaveChangesAsync();

        var updatedEvent = await context.AuditLogs.SingleAsync(entry =>
            entry.EventType == AuditLogEventTypes.DataChange &&
            entry.EntityName == nameof(User) &&
            entry.Method == "UPDATE");
        Assert.Contains("operator@example.com", updatedEvent.Details);
        Assert.Contains("renamed@example.com", updatedEvent.Details);

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        var deletedEvent = await context.AuditLogs.SingleAsync(entry =>
            entry.EventType == AuditLogEventTypes.DataChange &&
            entry.EntityName == nameof(User) &&
            entry.Method == "DELETE");
        Assert.Contains("renamed@example.com", deletedEvent.Details);
    }

    [Fact]
    public async Task SaveChangesAsyncWhenAcceptanceIsDeferredRestoresTrackedStateAfterAuditWrite()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);
        var room = new Room { Name = "A1" };
        context.Rooms.Add(room);

        await context.SaveChangesAsync(acceptAllChangesOnSuccess: false);

        Assert.Equal(EntityState.Added, context.Entry(room).State);
        Assert.Contains(context.AuditLogs, entry =>
            entry.EntityName == nameof(Room) &&
            entry.EntityId!.Contains(room.Id.ToString(CultureInfo.InvariantCulture)));
    }
}
