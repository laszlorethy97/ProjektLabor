namespace KeyManagement.Api.DTO
{
    public class RoomScheduleDTO
    {
        public List<TimeIntervalDTO> Reservations { get; set; } = new();
        public List<TimeIntervalDTO> Maintenances { get; set; } = new();
    }

    public class TimeIntervalDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}