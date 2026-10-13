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
    public async Task RecordKeyCheckoutAsyncFindsKeyByCode()
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
}
