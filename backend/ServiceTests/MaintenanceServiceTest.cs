using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class MaintenanceServiceTest
{
    [Fact]
    public async Task CreateMaintenanceAsync_WhenIntervalOverlapsReservation_ReturnsFailure()
    {
        await using var context = CreateContext();
        var (room, user) = await SeedRoomAndUserAsync(context);
        var reservationStart = DateTime.UtcNow.Date.AddDays(1).AddHours(10);
        context.Reservations.Add(new Reservation
        {
            Room = room,
            User = user,
            StartTime = reservationStart,
            EndTime = reservationStart.AddMinutes(45)
        });
        await context.SaveChangesAsync();

        var service = new MaintenanceService(context);
        var result = await service.CreateMaintenanceAsync(
            new CreateMaintenanceDTO
            {
                RoomId = room.Id,
                StartTime = reservationStart.AddMinutes(30),
                EndTime = reservationStart.AddHours(1)
            },
            user.Id);

        Assert.False(result.Success);
        Assert.Equal("The selected time interval is already occupied.", result.Error);
        Assert.Empty(await context.Maintenances.ToListAsync());
    }

    [Fact]
    public async Task CreateMaintenanceAsync_WhenIntervalOverlapsExistingMaintenance_ReturnsFailure()
    {
        await using var context = CreateContext();
        var (room, user) = await SeedRoomAndUserAsync(context);
        var maintenanceStart = DateTime.UtcNow.Date.AddDays(1).AddHours(10);
        context.Maintenances.Add(new Maintenance
        {
            Room = room,
            User = user,
            StartTime = maintenanceStart,
            EndTime = maintenanceStart.AddHours(1)
        });
        await context.SaveChangesAsync();

        var service = new MaintenanceService(context);
        var result = await service.CreateMaintenanceAsync(
            new CreateMaintenanceDTO
            {
                RoomId = room.Id,
                StartTime = maintenanceStart.AddMinutes(30),
                EndTime = maintenanceStart.AddHours(1).AddMinutes(30)
            },
            user.Id);

        Assert.False(result.Success);
        Assert.Equal("The selected time interval is already occupied.", result.Error);
        Assert.Single(await context.Maintenances.ToListAsync());
    }

    [Fact]
    public async Task CreateMaintenanceAsync_WhenIntervalOnlyTouchesReservation_CreatesMaintenance()
    {
        await using var context = CreateContext();
        var (room, user) = await SeedRoomAndUserAsync(context);
        var reservationStart = DateTime.UtcNow.Date.AddDays(1).AddHours(10);
        context.Reservations.Add(new Reservation
        {
            Room = room,
            User = user,
            StartTime = reservationStart,
            EndTime = reservationStart.AddMinutes(45)
        });
        await context.SaveChangesAsync();

        var service = new MaintenanceService(context);
        var result = await service.CreateMaintenanceAsync(
            new CreateMaintenanceDTO
            {
                RoomId = room.Id,
                StartTime = reservationStart.AddMinutes(45),
                EndTime = reservationStart.AddMinutes(90)
            },
            user.Id);

        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Single(await context.Maintenances.ToListAsync());
    }

    [Fact]
    public async Task CreateMaintenanceAsync_WhenStartTimeIsInThePast_ReturnsFailure()
    {
        await using var context = CreateContext();
        var (room, user) = await SeedRoomAndUserAsync(context);
        var service = new MaintenanceService(context);

        var result = await service.CreateMaintenanceAsync(
            new CreateMaintenanceDTO
            {
                RoomId = room.Id,
                StartTime = DateTime.UtcNow.AddMinutes(-2),
                EndTime = DateTime.UtcNow.AddMinutes(30)
            },
            user.Id);

        Assert.False(result.Success);
        Assert.Equal("Maintenance must start in the future.", result.Error);
        Assert.Empty(await context.Maintenances.ToListAsync());
    }

    [Fact]
    public async Task CreateMaintenanceAsync_WhenStartTimeIsNow_ReturnsFailure()
    {
        await using var context = CreateContext();
        var (room, user) = await SeedRoomAndUserAsync(context);
        var startTime = DateTime.UtcNow;
        var service = new MaintenanceService(context);

        var result = await service.CreateMaintenanceAsync(
            new CreateMaintenanceDTO
            {
                RoomId = room.Id,
                StartTime = startTime,
                EndTime = startTime.AddMinutes(30)
            },
            user.Id);

        Assert.False(result.Success);
        Assert.Equal("Maintenance must start in the future.", result.Error);
        Assert.Empty(await context.Maintenances.ToListAsync());
    }

    private static KeyManagementDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new KeyManagementDbContext(options);
    }

    private static async Task<(Room Room, User User)> SeedRoomAndUserAsync(
        KeyManagementDbContext context)
    {
        var building = new Building { Name = "Building 1" };
        var room = new Room { Name = "Room 1", Capacity = 20, Building = building };
        var user = new User { Email = "admin@example.com" };
        context.Rooms.Add(room);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return (room, user);
    }

}
