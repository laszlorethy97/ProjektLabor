using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class RoomServiceTest
{
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
                BuildingName = "Building 1",
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
                BuildingName = "BUILDING 1",
                Name = "Room 101",
                Capacity = 30
            },
            userId: 1);

        Assert.False(result.Success);
        Assert.Equal("A building with this name already exists.", result.Error);
        Assert.Empty(await context.Rooms.ToListAsync());
    }

    [Fact]
    public async Task CreateRoomAsyncFindsBuildingByNameIgnoringCaseAndWhitespace()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KeyManagementDbContext(options);
        var building = new Building { Id = 1, Name = "Building 1" };
        context.Buildings.Add(building);
        context.Users.Add(new User { Id = 1, Email = "admin@example.com" });
        await context.SaveChangesAsync();

        var service = new RoomService(context);
        var result = await service.CreateRoomAsync(
            new CreateRoomDTO
            {
                BuildingName = " BUILDING 1 ",
                Name = "Room 101",
                Capacity = 30
            },
            userId: 1);

        Assert.True(result.Success);
        var room = await context.Rooms.Include(item => item.Building).SingleAsync();
        Assert.Equal(building.Id, room.Building.Id);
    }

    [Fact]
    public async Task CreateRoomAsyncWhenBuildingNameDoesNotExistReturnsFailure()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KeyManagementDbContext(options);
        context.Users.Add(new User { Id = 1, Email = "admin@example.com" });
        await context.SaveChangesAsync();

        var service = new RoomService(context);
        var result = await service.CreateRoomAsync(
            new CreateRoomDTO
            {
                BuildingName = "Missing building",
                Name = "Room 101",
                Capacity = 30
            },
            userId: 1);

        Assert.False(result.Success);
        Assert.Equal("Building not found.", result.Error);
        Assert.Empty(await context.Rooms.ToListAsync());
    }
}
