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
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignRoles([FromBody] AssignRolesDto dto)
    {
        try
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier) || !int.TryParse(nameIdentifier, out int adminId))
            {
                return Forbid();
            }

            var userRoles = await _service.CreateUserRoleAsync(dto.Email, dto.RoleIds, adminId);
            return Ok(new
            {
                Email = dto.Email,
                Roles = userRoles.Select(userRole => new
                {
                    userRole.Role.Id,
                    userRole.Role.Type
                })
            });
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
