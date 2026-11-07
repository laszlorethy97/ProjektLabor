using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using KeyManagement.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    private readonly MaintenanceService _service;

    public MaintenanceController(MaintenanceService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateMaintenance(CreateMaintenanceDTO dto)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null || !int.TryParse(userId, out int adminId))
        {
            return Unauthorized();
        }

        var result = await _service.CreateMaintenanceAsync(dto, adminId);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(new { message = "Maintenance created successfully." });
    }

}