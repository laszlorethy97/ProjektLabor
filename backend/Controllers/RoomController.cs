using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;

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
}