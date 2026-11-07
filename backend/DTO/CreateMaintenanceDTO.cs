namespace KeyManagement.Api.DTO
{
    public class CreateMaintenanceDTO
    {
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}