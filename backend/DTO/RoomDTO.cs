namespace KeyManagement.Api.DTO
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Building { get; set; } = string.Empty;
    }
}