namespace KeyManagement.Api.DTO
{
    public class CreateRoomDTO
    {
        public int BuildingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public List<string> Equipments { get; set; } = new();
    }
}