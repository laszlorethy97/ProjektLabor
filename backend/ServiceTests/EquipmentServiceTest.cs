using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class EquipmentServiceTest
{
    private static KeyManagementDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new KeyManagementDbContext(options);
    }

    [Fact]
    public async Task CreateEquipmentAsyncFindsRoomByNameAndCreatesAllEquipment()
    {
        await using var context = CreateDbContext();
        var room = new Room { Id = 1, Name = "204-es terem", Capacity = 30 };
        var admin = new User { Id = 1, Email = "admin@example.com" };
        context.Rooms.Add(room);
        context.Users.Add(admin);
        await context.SaveChangesAsync();

        var service = new EquipmentService(context);
        var result = await service.CreateEquipmentAsync(
            new CreateEquipmentDTO
            {
                RoomName = " 204-ES TEREM ",
                EquipmentNames = [" projektor ", "tábla"]
            },
            admin.Id);

        Assert.True(result.Success);
        Assert.Null(result.Error);
        var equipment = await context.Equipments.Include(item => item.Room).ToListAsync();
        Assert.Equal(2, equipment.Count);
        Assert.All(equipment, item => Assert.Equal(room.Id, item.Room.Id));
        Assert.Contains(equipment, item => item.Name == "projektor");
        Assert.Contains(equipment, item => item.Name == "tábla");
    }

    [Fact]
    public async Task CreateEquipmentAsyncReturnsErrorWhenRoomDoesNotExist()
    {
        await using var context = CreateDbContext();
        context.Users.Add(new User { Id = 1, Email = "admin@example.com" });
        await context.SaveChangesAsync();

        var service = new EquipmentService(context);
        var result = await service.CreateEquipmentAsync(
            new CreateEquipmentDTO
            {
                RoomName = "Missing room",
                EquipmentNames = ["projektor"]
            },
            1);

        Assert.False(result.Success);
        Assert.Equal("Room not found.", result.Error);
        Assert.Empty(await context.Equipments.ToListAsync());
    }
}
