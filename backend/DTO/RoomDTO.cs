namespace KeyManagement.Api.DTO
{
    public class RoomDTO
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string BuildingName { get; set; } = string.Empty;
    }
}