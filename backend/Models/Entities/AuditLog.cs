using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public int? ActorUserId { get; set; }

    [StringLength(32)]
    public string EventType { get; set; } = AuditLogEventTypes.HttpRequest;

    [StringLength(256)]
    public string? ActorEmail { get; set; }

    [StringLength(256)]
    public string? ActorRoles { get; set; }

    [StringLength(10)]
    public string Method { get; set; } = string.Empty;

    [StringLength(512)]
    public string Resource { get; set; } = string.Empty;

    [StringLength(128)]
    public string? EntityName { get; set; }

    [StringLength(256)]
    public string? EntityId { get; set; }

    public string? Details { get; set; }

    [StringLength(64)]
    public string? CorrelationId { get; set; }

    public int StatusCode { get; set; }
    public DateTime OccurredAt { get; set; }

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(512)]
    public string? UserAgent { get; set; }

    public long DurationMilliseconds { get; set; }
}

public static class AuditLogEventTypes
{
    public const string HttpRequest = "HttpRequest";
    public const string DataChange = "DataChange";
    public const string Authentication = "Authentication";
}
