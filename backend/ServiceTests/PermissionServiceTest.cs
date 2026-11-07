using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class PermissionServiceTest
{
    private static DbContextOptions<KeyManagementDbContext> GetOptions()
    {
        return new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CreatePermissionAsyncCreatesPermissionForUserAndMasterKey()
    {
        using var context = new KeyManagementDbContext(GetOptions());
        var user = new User { Id = 1, Email = "user@test.hu" };
        var admin = new User { Id = 2, Email = "admin@test.hu" };
        var masterKey = new MasterKey { Id = 3, CreatedBy = admin };
        context.Users.AddRange(user, admin);
        context.MasterKeys.Add(masterKey);
        await context.SaveChangesAsync();

        var service = new PermissionService(context);
        var permission = await service.CreatePermissionAsync(
            new CreatePermissionDTO { UserId = user.Id, MasterKeyId = masterKey.Id },
            admin.Id);

        Assert.Equal(user.Id, permission.User.Id);
        Assert.Equal(masterKey.Id, permission.MasterKey.Id);
        Assert.Equal(admin.Id, permission.CreatedBy.Id);
        Assert.Single(await context.Permissions.ToListAsync());
    }

    [Fact]
    public async Task CreatePermissionAsyncWhenPermissionAlreadyExistsThrowsConflict()
    {
        using var context = new KeyManagementDbContext(GetOptions());
        var user = new User { Id = 1, Email = "user@test.hu" };
        var admin = new User { Id = 2, Email = "admin@test.hu" };
        var masterKey = new MasterKey { Id = 3, CreatedBy = admin };
        context.Users.AddRange(user, admin);
        context.MasterKeys.Add(masterKey);
        context.Permissions.Add(new Permission
        {
            User = user,
            MasterKey = masterKey,
            CreatedBy = admin
        });
        await context.SaveChangesAsync();

        var service = new PermissionService(context);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreatePermissionAsync(
                new CreatePermissionDTO { UserId = user.Id, MasterKeyId = masterKey.Id },
                admin.Id));

        Assert.Equal("User already has permission to use this master key.", exception.Message);
        Assert.Single(await context.Permissions.ToListAsync());
    }
}
