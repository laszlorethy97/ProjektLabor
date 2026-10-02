using System.Security.Claims;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly ReservationService _service;

    public ReservationController(ReservationService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "oktato")]
    public async Task<IActionResult> CreateReservationAsync(ReservationDTO reservationDTO)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _service.CreateReservationAsync(reservationDTO, userId);
            return Created();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
