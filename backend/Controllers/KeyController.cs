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
public class KeyController : ControllerBase
{
    private readonly KeyService _service;

    public KeyController(KeyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllKeys()
    {
        var keys = await _service.GetKeysAsync();
        return Ok(keys);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateKey([FromBody] CreateKeyDto dto)
    {
        try
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier) || !int.TryParse(nameIdentifier, out int adminId))
            {
                return Unauthorized("Invalid or missing admin token.");
            }

            var newKey = await _service.CreateKeyAsync(dto.Code, dto.RoomName, adminId);
            return CreatedAtAction(nameof(GetAllKeys), new { id = newKey.Id }, newKey);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
