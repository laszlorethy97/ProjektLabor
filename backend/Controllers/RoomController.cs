using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using KeyManagement.Api.DTO;
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

    [HttpGet]
    public async Task<ActionResult<List<Room>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("by-building")]
    public async Task<ActionResult<List<RoomDTO>>> GetRoomsByBuildingAsync()
    {
        return Ok(await _service.GetRoomsByBuildingAsync());
    }
}