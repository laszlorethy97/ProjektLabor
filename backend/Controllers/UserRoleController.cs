using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KeyManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserRoleController : ControllerBase
{
    private readonly UserRoleService _service;

    public UserRoleController(UserRoleService service)
    {
        _service = service;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRoles([FromBody] AssignRolesDto dto)
    {
        try
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier) || !int.TryParse(nameIdentifier, out int adminId))
            {
                return Unauthorized("Invalid or missing admin token.");
            }

            var result = await _service.CreateUserRoleAsync(dto.UserId, dto.RoleIds, adminId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
