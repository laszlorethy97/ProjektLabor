using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class KeyServiceTest
{
    [Fact]
    public async Task CreateKeyAsync_WhenCodeAlreadyExists_ThrowsInvalidOperationException()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KeyManagementDbContext(options);
        var room = new Room
        {
            Id = 1,
            Name = "Room 101",
            Capacity = 20,
            Building = new Building { Id = 1, Name = "Building 1" }
        };
        context.Keys.Add(new Key
        {
            Id = 1,
            Code = "K-101",
            Room = room,
            CreatedBy = new User { Id = 1 }
        });
        await context.SaveChangesAsync();

        var service = new KeyService(context);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateKeyAsync(" k-101 ", room.Name, createdByUserId: 1));

        Assert.Equal("A key with this code already exists.", exception.Message);
        Assert.Equal(1, await context.Keys.CountAsync());
    }
}
