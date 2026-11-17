using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "uzemeltetesi-igazgato")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _service;

    public AuditLogController(IAuditLogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditLogEntryResponse>>> GetRecentEventsAsync(
        [FromQuery] int limit = 200,
        CancellationToken cancellationToken = default)
    {
        if (limit is < 1 or > 500)
        {
            return BadRequest(new { message = "A limit értéke 1 és 500 közé essen." });
        }

        var result = await _service.GetEventsAsync(
            new AuditLogQuery { PageSize = limit },
            cancellationToken);
        return Ok(result.Items);
    }

    /// <summary>Returns a filtered page of audit events.</summary>
    [HttpGet("events")]
    [ProducesResponseType(typeof(AuditLogPageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuditLogPageResponse>> GetEventsAsync(
        [FromQuery] AuditLogQuery query,
        CancellationToken cancellationToken)
    {
        if ((query.FromDate is DateOnly fromDate &&
             query.ToDate is DateOnly toDate &&
             fromDate > toDate) ||
            query.ToDate == DateOnly.MaxValue ||
            (query.EventType is not null &&
             query.EventType is not (AuditLogEventTypes.HttpRequest or
                 AuditLogEventTypes.DataChange or AuditLogEventTypes.Authentication)))
        {
            return BadRequest(new { message = "Érvénytelen auditnapló-szűrő." });
        }

        return Ok(await _service.GetEventsAsync(query, cancellationToken));
    }

    [HttpGet("overdue-keys")]
    public async Task<ActionResult<List<OverdueKeyDTO>>> GetOverdueKeysAsync(
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetOverdueKeysAsync(DateTime.UtcNow, cancellationToken));
    }

    [HttpGet("traffic")]
    public async Task<ActionResult<TrafficReportDTO>> GetTrafficReportAsync(
        [FromQuery] string period = "daily",
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        if (period is not ("daily" or "weekly"))
        {
            return BadRequest(new { message = "A period értéke daily vagy weekly lehet." });
        }

        var endDate = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = from ?? endDate.AddDays(period == "daily" ? -6 : -83);
        var maximumRangeDays = period == "daily" ? 366 : 1096;

        if (startDate > endDate ||
            endDate == DateOnly.MaxValue ||
            endDate.DayNumber - startDate.DayNumber >= maximumRangeDays)
        {
            return BadRequest(new { message = $"A jelentés legfeljebb {maximumRangeDays} napot fedhet le." });
        }

        return Ok(await _service.GetTrafficReportAsync(
            period,
            startDate,
            endDate,
            cancellationToken));
    }

    /// <summary>Returns reservation counts and booked hours grouped by room and period.</summary>
    [HttpGet("room-utilization")]
    [ProducesResponseType(typeof(RoomUtilizationReportDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomUtilizationReportDTO>> GetRoomUtilizationReportAsync(
        [FromQuery] string period = "daily",
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        if (period is not ("daily" or "weekly"))
        {
            return BadRequest(new { message = "A period értéke daily vagy weekly lehet." });
        }

        var endDate = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = from ?? endDate.AddDays(period == "daily" ? -6 : -83);
        var maximumRangeDays = period == "daily" ? 366 : 1096;

        if (startDate > endDate ||
            endDate == DateOnly.MaxValue ||
            endDate.DayNumber - startDate.DayNumber >= maximumRangeDays)
        {
            return BadRequest(new { message = $"A jelentés legfeljebb {maximumRangeDays} napot fedhet le." });
        }

        return Ok(await _service.GetRoomUtilizationReportAsync(
            period,
            startDate,
            endDate,
            cancellationToken));
    }
}
