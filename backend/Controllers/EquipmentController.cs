using KeyManagement.Api.DTO;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [HttpGet("by-room-id/{roomId}")]
        public async Task<ActionResult<List<EquipmentDTO>>> GetEquipmentsByRoomId(int roomId)
        {
            return Ok(await _service.GetEquipmentsByRoomId(roomId));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateEquipmentAsync(CreateEquipmentDTO dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null || !int.TryParse(userId, out int adminId))
                return Unauthorized();

            var result = await _service.CreateEquipmentAsync(dto, adminId);

            if (!result.Success)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = "Equipment created successfully." });
        }
    }
}