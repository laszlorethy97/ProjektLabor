using KeyManagement.Api.Services;
using KeyManagement.Api.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly PermissionService _service;

    public PermissionController(PermissionService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreatePermissionAsync(CreatePermissionDTO dto)
    {
        if (!int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out int createdByUserId))
        {
            return Unauthorized();
        }

        try
        {
            var permission = await _service.CreatePermissionAsync(dto, createdByUserId);
            return StatusCode(StatusCodes.Status201Created, new
            {
                permission.Id,
                UserId = dto.UserId,
                MasterKeyId = dto.MasterKeyId,
                permission.CreatedAt
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
