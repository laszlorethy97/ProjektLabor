using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly EquipmentService _service;

    public EquipmentController(EquipmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Equipment>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("by-room-id/{roomId}")]
    public async Task<ActionResult<List<Equipment>>> GetEquipmentsByRoomId(int roomId)
    {
        return Ok(await _service.GetEquipmentsByRoomId(roomId));
    }
}
}