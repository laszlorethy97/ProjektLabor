using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class KeyTransactionServiceTest
{
    [Fact]
    public async Task RecordKeyCheckoutAsync_FindsKeyByCode()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var user = new User { Id = 1, Email = "user@example.com", PinCode = "0042" };
        var startedByUser = new User { Id = 2, Email = "porter@example.com" };
        var building = new Building { Id = 1, Name = "Building 1" };
        var room = new Room { Id = 1, Name = "Room 1", Building = building };
        var key = new Key { Id = 1, Code = "KEY-001", CreatedBy = startedByUser, Room = room };
        room.Keys.Add(key);

        context.Users.Add(user);
        context.Users.Add(startedByUser);
        context.Keys.Add(key);
        context.Reservations.Add(new Reservation
        {
            Id = 1,
            StartTime = DateTime.UtcNow.AddMinutes(-5),
            EndTime = DateTime.UtcNow.AddHours(1),
            Room = room,
            User = user,
        });
        await context.SaveChangesAsync();
        var service = new KeyTransactionService(context);
        var transaction = await service.RecordKeyCheckoutAsync(
            new KeyTransactionDTO
            {
                Email = user.Email,
                PinCode = user.PinCode,
                KeyCode = " KEY-001 ",
            },
            startedByUser.Id);

        Assert.Equal(key.Id, transaction.Key.Id);
        Assert.Equal("KEY-001", transaction.Key.Code);
    }

    [Fact]
    public async Task RecordKeyCheckoutAsync_StartUserNotFound()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var user = new User { Id = 1, Email = "user@example.com", PinCode = "0042" };
        
        int nonExistentUserId = 999;
        var building = new Building { Id = 1, Name = "Building 1" };
        var room = new Room { Id = 1, Name = "Room 1", Building = building };
        var key = new Key { Id = 1, Code = "KEY-001", Room = room };
        room.Keys.Add(key);

        context.Users.Add(user);
        context.Keys.Add(key);
        context.Reservations.Add(new Reservation
        {
            Id = 1,
            StartTime = DateTime.UtcNow.AddMinutes(-5),
            EndTime = DateTime.UtcNow.AddHours(1),
            Room = room,
            User = user,
        });
        await context.SaveChangesAsync();

        var service = new KeyTransactionService(context);
            var dto =new KeyTransactionDTO
            {
                Email = user.Email,
                PinCode = user.PinCode,
                KeyCode = "KEY-001",
            };
            

        var exception =await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.RecordKeyCheckoutAsync(dto, nonExistentUserId);
            });
            Assert.Equal("Started by user not found.", exception.Message);

            var transactionCount = await context.KeyTransactions.CountAsync();
            Assert.Equal(0, transactionCount);
    }

    [Fact]
    public async Task RecordKeyReturnAsync_TransactionSuccesfullyEnded()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var user = new User { Id = 1, Email = "user@example.com", PinCode = "0042" };
        var startedByUser = new User { Id = 2, Email = "porter@example.com" };
        var endedByUser = new User { Id = 3, Email = "porter2@example.com" };
        var building = new Building { Id = 1, Name = "Building 1" };
        var room = new Room { Id = 1, Name = "Room 1", Building = building };
        var key = new Key { Id = 1, Code = "KEY-003", Room = room };
        room.Keys.Add(key);

        context.Users.Add(user);
        context.Users.Add(startedByUser);
        context.Users.Add(endedByUser);
        context.Keys.Add(key);
        context.Reservations.Add(new Reservation
        {
            Id = 1,
            StartTime = DateTime.UtcNow.AddMinutes(-5),
            EndTime = DateTime.UtcNow.AddHours(1),
            Room = room,
            User = user,
        });

        var activeTransaction = new KeyTransaction
        {
            Id = 1,
            Key = key,
            User = user,
            StartedByUser = startedByUser,
            StartedAt = DateTime.UtcNow.AddHours(-1)
        };
        context.KeyTransactions.Add(activeTransaction);
        await context.SaveChangesAsync();

        var service = new KeyTransactionService(context);
            var dto =new KeyTransactionDTO
            {
                Email = user.Email,
                PinCode = user.PinCode,
                KeyCode = "KEY-003",
            };
        
        var ReturnTransaction = await service.RecordKeyReturnAsync(dto, endedByUser.Id);
            Assert.NotNull(ReturnTransaction.EndedAt);
            Assert.Equal(ReturnTransaction.EndedByUser.Id, endedByUser.Id);
            Assert.Equal(ReturnTransaction.Key.Id, key.Id);
    }

    [Fact]
    public async Task RecordKeyReturnAsync_EndUserNotFound()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var user = new User { Id = 1, Email = "user@example.com", PinCode = "0042" };
        var startedByUser = new User { Id = 2, Email = "porter@example.com" };
        int nonExistentUserId = 999;
        var building = new Building { Id = 1, Name = "Building 1" };
        var room = new Room { Id = 1, Name = "Room 1", Building = building };
        var key = new Key { Id = 1, Code = "KEY-004", Room = room };
        room.Keys.Add(key);

        context.Users.Add(user);
        context.Users.Add(startedByUser);
        context.Keys.Add(key);
        context.Reservations.Add(new Reservation
        {
            Id = 1,
            StartTime = DateTime.UtcNow.AddMinutes(-5),
            EndTime = DateTime.UtcNow.AddHours(1),
            Room = room,
            User = user,
        });

        var activeTransaction = new KeyTransaction
        {
            Id = 1,
            Key = key,
            User = user,
            StartedByUser = startedByUser,
            StartedAt = DateTime.UtcNow.AddHours(-1)
        };
        context.KeyTransactions.Add(activeTransaction);
        await context.SaveChangesAsync();
        
        var service = new KeyTransactionService(context);
            var dto =new KeyTransactionDTO
            {
                Email = user.Email,
                PinCode = user.PinCode,
                KeyCode = "KEY-004",
            };
        
         var exception =await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.RecordKeyReturnAsync(dto, nonExistentUserId);
            });
            Assert.Equal("Ended by user not found.", exception.Message);   
    }
    [Fact]
    public async Task RecordKeyReturnAsync_KeyTransactionNotFound()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var user = new User { Id = 1, Email = "user@example.com", PinCode = "0042" };
        var startedByUser = new User { Id = 2, Email = "porter@example.com" };
        var endedByUser = new User { Id = 3, Email = "porter2@example.com" };
        var building = new Building { Id = 1, Name = "Building 1" };
        var room = new Room { Id = 1, Name = "Room 1", Building = building };
        var key = new Key { Id = 1, Code = "KEY-004", Room = room };
        room.Keys.Add(key);

        context.Users.Add(user);
        context.Users.Add(startedByUser);
        context.Keys.Add(key);
        context.Reservations.Add(new Reservation
        {
            Id = 1,
            StartTime = DateTime.UtcNow.AddMinutes(-5),
            EndTime = DateTime.UtcNow.AddHours(1),
            Room = room,
            User = user,
        });
        await context.SaveChangesAsync();
        
        var service = new KeyTransactionService(context);
            var dto =new KeyTransactionDTO
            {
                Email = user.Email,
                PinCode = user.PinCode,
                KeyCode = "KEY-004",
            };
        
         var exception =await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.RecordKeyReturnAsync(dto, endedByUser.Id);
            });
            Assert.Equal("No open transaction found for this key.", exception.Message);   
    }

    [Fact]
    public async Task RecordKeyReturnAsync_CheckoutByDifferentUser()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var user = new User { Id = 1, Email = "user@example.com", PinCode = "0042" };
        var user2 = new User { Id = 4, Email = "user2@example.com", PinCode = "0043" };
        var startedByUser = new User { Id = 2, Email = "porter@example.com" };
        var endedByUser = new User { Id = 3, Email = "porter2@example.com" };
        var building = new Building { Id = 1, Name = "Building 1" };
        var room = new Room { Id = 1, Name = "Room 1", Building = building };
        var key = new Key { Id = 1, Code = "KEY-004", Room = room };
        room.Keys.Add(key);

        context.Users.Add(user);
        context.Users.Add(user2);
        context.Users.Add(startedByUser);
        context.Users.Add(endedByUser);
        context.Keys.Add(key);
        context.Reservations.Add(new Reservation
        {
            Id = 1,
            StartTime = DateTime.UtcNow.AddMinutes(-5),
            EndTime = DateTime.UtcNow.AddHours(1),
            Room = room,
            User = user,
        });

        var activeTransaction = new KeyTransaction
        {
            Id = 1,
            Key = key,
            User = user,
            StartedByUser = startedByUser,
            StartedAt = DateTime.UtcNow.AddHours(-1)
        };
        context.KeyTransactions.Add(activeTransaction);
        await context.SaveChangesAsync();
        
        var service = new KeyTransactionService(context);
            var dto =new KeyTransactionDTO
            {
                Email = user.Email,
                PinCode = user.PinCode,
                KeyCode = "KEY-004",
            };
            var dto2 =new KeyTransactionDTO
            {
                Email = user2.Email,
                PinCode = user2.PinCode,
                KeyCode = "KEY-004",
            };
        
         var exception =await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.RecordKeyReturnAsync(dto2, endedByUser.Id);
            });
            Assert.Equal("This key was checked out by a different user.", exception.Message);   
    }

}
