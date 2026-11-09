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

    /// <summary>Returns reservations and maintenance intervals for a room.</summary>
    [HttpGet("schedule/{roomId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(RoomScheduleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomScheduleDTO>> GetRoomScheduleAsync(
        int roomId,
        CancellationToken cancellationToken)
    {
        if (roomId <= 0)
        {
            return BadRequest(new { message = "Room ID must be a positive integer." });
        }

        var result = await _service.GetRoomScheduleAsync(roomId, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = "Room not found." });
        }

        return Ok(result);
    }

    [HttpPost("schedule")]
    [Authorize]
    public async Task<ActionResult<RoomScheduleDTO>> GetRoomScheduleAsync(
        RoomScheduleRequestDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetRoomScheduleAsync(dto.RoomId, cancellationToken);

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
            return BadRequest(new { message = result.Error });

        return Ok(new { message = "Room created successfully." });
    }
}