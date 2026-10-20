using KeyManagement.Api.DTO;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasterKeyTransactionController : ControllerBase
{
    private readonly MasterKeyTransactionService _service;

    public MasterKeyTransactionController(
        MasterKeyTransactionService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "portas")]
    public async Task<IActionResult> RecordMasterKeyCheckoutAsync(
        MasterKeyTransactionDTO dto)
    {
        if (!int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _service.RecordMasterKeyCheckoutAsync(dto, userId);
            return Created();
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

    [HttpPost("return")]
    [Authorize(Roles = "portas")]
    public async Task<IActionResult> RecordMasterKeyReturnAsync(
        MasterKeyTransactionDTO dto)
    {
        if (!int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _service.RecordMasterKeyReturnAsync(dto, userId);
            return NoContent();
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
