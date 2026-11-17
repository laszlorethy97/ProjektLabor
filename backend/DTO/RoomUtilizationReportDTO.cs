namespace KeyManagement.Api.DTO;

/// <summary>Summarizes room reservations for a requested report period.</summary>
public sealed record RoomUtilizationReportDTO(
    string Period,
    DateOnly From,
    DateOnly To,
    IReadOnlyList<RoomUtilizationEntryDTO> Entries);

/// <summary>Reports reservations and booked hours for one room and period.</summary>
public sealed record RoomUtilizationEntryDTO(
    int RoomId,
    string RoomName,
    string BuildingName,
    DateOnly PeriodStart,
    int ReservationCount,
    decimal ReservedHours);
