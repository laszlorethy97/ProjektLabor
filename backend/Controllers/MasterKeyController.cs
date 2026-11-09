using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KeyManagement.Api.Controllers;

[Authorize(Roles = "admin")]
[ApiController]
[Route("api/[controller]")]
public class MasterKeyController : ControllerBase
{
    private readonly MasterKeyService _service;

    public MasterKeyController(MasterKeyService service)
    {
        _service = service;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateMasterKey()
    {
        if (!int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out int adminId))
        {
            return Unauthorized();
        }

        try
        {
            var newMasterKey = await _service.CreateMasterKeyAsync(adminId);
            return StatusCode(StatusCodes.Status201Created, new
            {
                newMasterKey.Id,
                newMasterKey.CreatedAt
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
