using KeyManagement.Api.Models.Entities;
using System.Security.Claims;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KeyTransactionController : ControllerBase
{
    private readonly KeyTransactionService _service;

    public KeyTransactionController(KeyTransactionService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize]
    [Authorize(Roles = "portas")]
    public async Task<IActionResult> RecordKeyCheckoutAsync(KeyTransactionDTO keyTransactionDTO)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _service.RecordKeyCheckoutAsync(keyTransactionDTO, userId);
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
    public async Task<IActionResult> RecordKeyReturnAsync(KeyTransactionDTO keyTransactionDTO)
    {
        if (!int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _service.RecordKeyReturnAsync(
                keyTransactionDTO,
                userId);

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