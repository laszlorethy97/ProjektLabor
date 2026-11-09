using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class RoomServiceTest
{
    [Fact]
    public async Task GetRoomScheduleAsyncReturnsReservationsAndMaintenancesInStartOrder()
    {
        await using var context = CreateContext();
        var room = new Room
        {
            Id = 7,
            Name = "A101",
            Capacity = 20,
            Building = new Building { Id = 1, Name = "A" }
        };
        var user = new User { Id = 1, Email = "teacher@example.com" };
        var later = DateTime.SpecifyKind(new DateTime(2030, 4, 2, 12, 0, 0), DateTimeKind.Utc);
        var earlier = later.AddHours(-2);
        context.Rooms.Add(room);
        context.Users.Add(user);
        context.Reservations.AddRange(
            new Reservation { Room = room, User = user, StartTime = later, EndTime = later.AddMinutes(45) },
            new Reservation { Room = room, User = user, StartTime = earlier, EndTime = earlier.AddMinutes(45) });
        context.Maintenances.Add(new Maintenance
        {
            Room = room,
            User = user,
            StartTime = earlier.AddDays(1),
            EndTime = earlier.AddDays(1).AddHours(1)
        });
        await context.SaveChangesAsync();

        var result = await new RoomService(context).GetRoomScheduleAsync(room.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Reservations.Count);
        Assert.Equal(new DateTimeOffset(earlier), result.Reservations[0].StartTime);
        Assert.Equal(new DateTimeOffset(later), result.Reservations[1].StartTime);
        Assert.Single(result.Maintenances);
        Assert.Equal(TimeSpan.Zero, result.Maintenances[0].StartTime.Offset);
    }

    [Fact]
    public async Task GetRoomScheduleAsyncReturnsNullWhenRoomDoesNotExist()
    {
        await using var context = CreateContext();

        var result = await new RoomService(context).GetRoomScheduleAsync(404);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("Room 101")]
    [InlineData(" room 101 ")]
    [InlineData("ROOM 101")]
    public async Task CreateRoomAsync_WhenNameAlreadyExists_ReturnsFailure(string requestedName)
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KeyManagementDbContext(options);
        context.Buildings.Add(new Building { Id = 1, Name = "Building 1" });
        context.Rooms.Add(new Room
        {
            Id = 1,
            Name = "Room 101",
            Capacity = 20,
            Building = new Building { Id = 2, Name = "Building 2" }
        });
        await context.SaveChangesAsync();

        var service = new RoomService(context);
        var result = await service.CreateRoomAsync(
            new CreateRoomDTO
            {
                BuildingId = 1,
                Name = requestedName,
                Capacity = 30
            },
            userId: 1);

        Assert.False(result.Success);
        Assert.Equal("A room with this name already exists.", result.Error);
        Assert.Equal(1, await context.Rooms.CountAsync());
    }

    [Fact]
    public async Task CreateRoomAsync_WhenBuildingNameIsNotUnique_ReturnsFailure()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KeyManagementDbContext(options);
        context.Buildings.AddRange(
            new Building { Id = 1, Name = "Building 1" },
            new Building { Id = 2, Name = "building 1" });
        await context.SaveChangesAsync();

        var service = new RoomService(context);
        var result = await service.CreateRoomAsync(
            new CreateRoomDTO
            {
                BuildingId = 1,
                Name = "Room 101",
                Capacity = 30
            },
            userId: 1);

        Assert.False(result.Success);
        Assert.Equal("A building with this name already exists.", result.Error);
        Assert.Empty(await context.Rooms.ToListAsync());
    }

    private static KeyManagementDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new KeyManagementDbContext(options);
    }
}
