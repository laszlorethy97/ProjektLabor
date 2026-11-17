using KeyManagement.Api.DTO;

namespace KeyManagement.Api.Services;

public interface IAuditLogService
{
    Task<AuditLogPageResponse> GetEventsAsync(
        AuditLogQuery query,
        CancellationToken cancellationToken);

    Task<List<OverdueKeyDTO>> GetOverdueKeysAsync(
        DateTime now,
        CancellationToken cancellationToken);

    Task<TrafficReportDTO> GetTrafficReportAsync(
        string period,
        DateOnly from,
        DateOnly endDate,
        CancellationToken cancellationToken);

    Task<RoomUtilizationReportDTO> GetRoomUtilizationReportAsync(
        string period,
        DateOnly from,
        DateOnly endDate,
        CancellationToken cancellationToken);
}
