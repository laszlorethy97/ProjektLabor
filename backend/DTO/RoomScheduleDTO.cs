namespace KeyManagement.Api.DTO;

/// <summary>Contains all reservation and maintenance intervals for a room.</summary>
public sealed record RoomScheduleDTO(
    IReadOnlyList<TimeIntervalDTO> Reservations,
    IReadOnlyList<TimeIntervalDTO> Maintenances);

/// <summary>Represents a room occupancy interval in UTC.</summary>
public sealed record TimeIntervalDTO(DateTimeOffset StartTime, DateTimeOffset EndTime);