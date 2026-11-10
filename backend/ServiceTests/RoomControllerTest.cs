using System.Security.Claims;
using KeyManagement.Api.Controllers;
using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class RoomControllerTest
{
    [Fact]
    public async Task CreateRoomAsyncWhenRoomNameAlreadyExistsReturnsConflictWithMessage()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KeyManagementDbContext(options);
        var building = new Building { Id = 1, Name = "Building 1" };
        context.Buildings.Add(building);
        context.Rooms.Add(new Room
        {
            Id = 1,
            Name = "Room 101",
            Capacity = 30,
            Building = building
        });
        context.Users.Add(new User { Id = 1, Email = "admin@example.com" });
        await context.SaveChangesAsync();

        var controller = new RoomController(new RoomService(context))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, "1")
                    ], "test"))
                }
            }
        };

        var result = await controller.CreateRoomAsync(new CreateRoomDTO
        {
            BuildingName = "Building 1",
            Name = " room 101 ",
            Capacity = 25
        });

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);
        Assert.Equal(
            "A room with this name already exists.",
            conflict.Value?.GetType().GetProperty("message")?.GetValue(conflict.Value));
    }
}
