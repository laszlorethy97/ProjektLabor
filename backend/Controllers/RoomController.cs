using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly RoomService _service;

    public RoomController(RoomService service)
    {
        _service = service;
    }



    [HttpGet("by-building")]
    [Authorize(Roles = "oktato,admin")]
    public async Task<ActionResult<List<RoomDTO>>> GetRoomsByBuildingAsync()
    {
        return Ok(await _service.GetRoomsByBuildingAsync());
    }

    [HttpPost("schedule")]
    [Authorize]
    public async Task<ActionResult<RoomScheduleDTO>> GetRoomScheduleAsync(
        RoomScheduleRequestDTO dto)
    {
        var result = await _service.GetRoomScheduleAsync(dto.RoomId);

        if (result == null)
        {
            return NotFound(new { message = "Room not found." });
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateRoomAsync(CreateRoomDTO dto)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null || !int.TryParse(userId, out int adminId))
            return Unauthorized();

        var result = await _service.CreateRoomAsync(dto, adminId);

        if (!result.Success)
        {
            var error = result.Error;
            if (error == "Building not found.")
                return NotFound(new { message = error });

            if (error is "A room with this name already exists." or "A building with this name already exists.")
                return Conflict(new { message = error });

            return BadRequest(new { message = error });
        }

        return StatusCode(StatusCodes.Status201Created, new { message = "Room created successfully." });
    }
}