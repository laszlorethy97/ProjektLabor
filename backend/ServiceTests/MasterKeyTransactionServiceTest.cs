using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class MasterKeyTransactionServiceTest
{
    private DbContextOptions<KeyManagementDbContext> _GetOptions()
    {
        return new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task RecordMasterKeyCheckoutAsync_HappyPath_CreatesTransaction()
    {
        using var context = new KeyManagementDbContext(_GetOptions());
        var user = new User { Id = 1, Email = "tanar@test.hu", PinCode = "1234" };
        var porter = new User { Id = 2, Email = "portas@test.hu" };
        var masterKey = new MasterKey { Id = 10 };
        var permission = new Permission { Id = 1, User = user, MasterKey = masterKey };

        context.Users.AddRange(user, porter);
        context.MasterKeys.Add(masterKey);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var service = new MasterKeyTransactionService(context);
        var dto = new MasterKeyTransactionDTO { Email = user.Email, PinCode = user.PinCode, MasterKeyId = masterKey.Id };

        var transaction = await service.RecordMasterKeyCheckoutAsync(dto, porter.Id);

        Assert.NotNull(transaction);
        Assert.Equal(masterKey.Id, transaction.MasterKey.Id);
        Assert.Equal(user.Id, transaction.User.Id);
    }

    [Fact]
    public async Task RecordMasterKeyCheckoutAsync_IfNoPermission_ThrowsInvalidOperationException()
    {

        using var context = new KeyManagementDbContext(_GetOptions());
        var user = new User { Id = 1, Email = "tanar@test.hu", PinCode = "1234" };
        var porter = new User { Id = 2, Email = "portas@test.hu" };
        var masterKey = new MasterKey { Id = 10 };

        context.Users.AddRange(user, porter);
        context.MasterKeys.Add(masterKey);
        
        await context.SaveChangesAsync();

        var service = new MasterKeyTransactionService(context);
        var dto = new MasterKeyTransactionDTO { Email = user.Email, PinCode = user.PinCode, MasterKeyId = masterKey.Id };

        
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.RecordMasterKeyCheckoutAsync(dto, porter.Id);
        });
        Assert.Equal("User does not have permission to use this master key.", ex.Message);
    }

    [Fact]
    public async Task RecordMasterKeyCheckoutAsync_IfAlreadyCheckedOut_ThrowsInvalidOperationException()
    {
        
        using var context = new KeyManagementDbContext(_GetOptions());
        var user = new User { Id = 1, Email = "tanar@test.hu", PinCode = "1234" };
        var porter = new User { Id = 2, Email = "portas@test.hu" };
        var masterKey = new MasterKey { Id = 10 };
        var permission = new Permission { Id = 1, User = user, MasterKey = masterKey };

        context.Users.AddRange(user, porter);
        context.MasterKeys.Add(masterKey);
        context.Permissions.Add(permission);
        
        
        context.MasterKeyTransactions.Add(new MasterKeyTransaction
        {
            Id = 1, MasterKey = masterKey, User = user, StartedByUser = porter, StartedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new MasterKeyTransactionService(context);
        var dto = new MasterKeyTransactionDTO { Email = user.Email, PinCode = user.PinCode, MasterKeyId = masterKey.Id };

        
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.RecordMasterKeyCheckoutAsync(dto, porter.Id);
        });
        Assert.Equal("Master key is already checked out.", ex.Message);
    }

    [Fact]
    public async Task RecordMasterKeyReturnAsync_HappyPath_EndsTransaction()
    {
        
        using var context = new KeyManagementDbContext(_GetOptions());
        var user = new User { Id = 1, Email = "tanar@test.hu", PinCode = "1234" };
        var porter = new User { Id = 2, Email = "portas@test.hu" };
        var masterKey = new MasterKey { Id = 10 };

        context.Users.AddRange(user, porter);
        context.MasterKeys.Add(masterKey);

        var activeTx = new MasterKeyTransaction
        {
            Id = 1, MasterKey = masterKey, User = user, StartedByUser = porter, StartedAt = DateTime.UtcNow.AddHours(-1)
        };
        context.MasterKeyTransactions.Add(activeTx);
        await context.SaveChangesAsync();

        var service = new MasterKeyTransactionService(context);
        var dto = new MasterKeyTransactionDTO { Email = user.Email, PinCode = user.PinCode, MasterKeyId = masterKey.Id };

        
        var transaction = await service.RecordMasterKeyReturnAsync(dto, porter.Id);

        
        Assert.NotNull(transaction.EndedAt);
        Assert.Equal(porter.Id, transaction.EndedByUser.Id);
    }

    [Fact]
    public async Task RecordMasterKeyReturnAsync_IfDifferentUserTriesToReturn_ThrowsInvalidOperationException()
    {
        
        using var context = new KeyManagementDbContext(_GetOptions());
        var userWhoCheckedOut = new User { Id = 1, Email = "tanar1@test.hu", PinCode = "1234" };
        var userWhoTriesToReturn = new User { Id = 3, Email = "tanar2@test.hu", PinCode = "5678" };
        var porter = new User { Id = 2, Email = "portas@test.hu" };
        var masterKey = new MasterKey { Id = 10 };

        context.Users.AddRange(userWhoCheckedOut, userWhoTriesToReturn, porter);
        context.MasterKeys.Add(masterKey);

        
        var activeTx = new MasterKeyTransaction
        {
            Id = 1, MasterKey = masterKey, User = userWhoCheckedOut, StartedByUser = porter, StartedAt = DateTime.UtcNow.AddHours(-1)
        };
        context.MasterKeyTransactions.Add(activeTx);
        await context.SaveChangesAsync();

        var service = new MasterKeyTransactionService(context);
        
        var dto = new MasterKeyTransactionDTO { Email = userWhoTriesToReturn.Email, PinCode = userWhoTriesToReturn.PinCode, MasterKeyId = masterKey.Id };

        
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.RecordMasterKeyReturnAsync(dto, porter.Id);
        });
        Assert.Equal("This master key was checked out by a different user.", ex.Message);
    }

    [Fact]
    public async Task RecordMasterKeyReturnAsync_IfNoOpenTransaction_ThrowsKeyNotFoundException()
    {
        
        using var context = new KeyManagementDbContext(_GetOptions());
        var user = new User { Id = 1, Email = "tanar@test.hu", PinCode = "1234" };
        var porter = new User { Id = 2, Email = "portas@test.hu" };
        var masterKey = new MasterKey { Id = 10 };

        context.Users.AddRange(user, porter);
        context.MasterKeys.Add(masterKey);
        
        await context.SaveChangesAsync();

        var service = new MasterKeyTransactionService(context);
        var dto = new MasterKeyTransactionDTO { Email = user.Email, PinCode = user.PinCode, MasterKeyId = masterKey.Id };

        
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await service.RecordMasterKeyReturnAsync(dto, porter.Id);
        });
        Assert.Equal("No open transaction found for this master key.", ex.Message);
    }
}