using KeyManagement.Api.DTO;
using KeyManagement.Api.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly TicketService _service;

    public TicketController(TicketService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "portas")]
    public async Task<IActionResult> CreateTicketAsync(CreateTicketDTO dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(
            userIdClaim,
            System.Globalization.CultureInfo.InvariantCulture);
                var ticket = await _service.CreateTicketAsync(
                    dto.Description,
                    userId);

        return CreatedAtAction(
            nameof(CreateTicketAsync),
            new { id = ticket.Id },
            ticket);
    }
}