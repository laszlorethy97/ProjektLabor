using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO;

/// <summary>Filters the paged audit-event query.</summary>
public sealed record AuditLogQuery
{
    /// <summary>One-based page number.</summary>
    [Range(1, 100000)]
    public int Page { get; init; } = 1;

    /// <summary>Number of events per page, from 1 to 100.</summary>
    [Range(1, 100)]
    public int PageSize { get; init; } = 50;

    /// <summary>Optional event type filter.</summary>
    [StringLength(32)]
    public string? EventType { get; init; }

    /// <summary>Optional actor user ID filter.</summary>
    [Range(1, int.MaxValue)]
    public int? ActorUserId { get; init; }

    /// <summary>Optional case-insensitive search across event identity fields.</summary>
    [StringLength(128)]
    public string? Search { get; init; }

    /// <summary>Optional inclusive start date in UTC.</summary>
    public DateOnly? FromDate { get; init; }

    /// <summary>Optional inclusive end date in UTC.</summary>
    public DateOnly? ToDate { get; init; }
}

/// <summary>A page of audit events and its pagination metadata.</summary>
public sealed record AuditLogPageResponse(
    IReadOnlyList<AuditLogEntryResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);

/// <summary>One security or data-change audit event.</summary>
public sealed record AuditLogEntryResponse(
    long Id,
    string EventType,
    int? ActorUserId,
    string? ActorEmail,
    string? ActorRoles,
    string Method,
    string Resource,
    int StatusCode,
    DateTimeOffset OccurredAt,
    string? IpAddress,
    string? UserAgent,
    long DurationMilliseconds,
    string? EntityName,
    string? EntityId,
    string? Details,
    string? CorrelationId);

public class OverdueKeyDTO
{
    public int TransactionId { get; set; }
    public string KeyCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public int BorrowerUserId { get; set; }
    public string BorrowerEmail { get; set; } = string.Empty;
    public DateTime CheckedOutAt { get; set; }
    public DateTime DueAt { get; set; }
}

public class TrafficReportDTO
{
    public string Period { get; set; } = string.Empty;
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public List<TrafficReportEntryDTO> Entries { get; set; } = new();
}

public class TrafficReportEntryDTO
{
    public DateOnly PeriodStart { get; set; }
    public int Checkouts { get; set; }
    public int Returns { get; set; }
}
