using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class MasterKeyServiceTest
{
    [Fact]
    public async Task GrantMasterKeyPermissionAsyncFindsUserByEmailIgnoringCaseAndWhitespace()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KeyManagementDbContext(options);
        var user = new User { Id = 1, Email = "user@example.com" };
        var admin = new User { Id = 2, Email = "admin@example.com" };
        var masterKey = new MasterKey { Id = 3, CreatedBy = admin };
        context.Users.AddRange(user, admin);
        context.MasterKeys.Add(masterKey);
        await context.SaveChangesAsync();

        var service = new MasterKeyService(context);
        var permission = await service.GrantMasterKeyPermissionAsync(
            " USER@EXAMPLE.COM ",
            masterKey.Id,
            admin.Id);

        Assert.Equal(user.Id, permission.User.Id);
        Assert.Equal(masterKey.Id, permission.MasterKey.Id);
        Assert.Equal(admin.Id, permission.CreatedBy.Id);
    }

    [Fact]
    public async Task GrantMasterKeyPermissionAsyncThrowsWhenPermissionAlreadyExists()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KeyManagementDbContext(options);
        var user = new User { Id = 1, Email = "user@example.com" };
        var admin = new User { Id = 2, Email = "admin@example.com" };
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

        var service = new MasterKeyService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GrantMasterKeyPermissionAsync(user.Email, masterKey.Id, admin.Id));
    }
}
