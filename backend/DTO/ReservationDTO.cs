namespace KeyManagement.Api.DTO
{
    public class ReservationDTO
    {
        public int RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public List<int> EquipmentIds { get; set; } = new List<int>();
    }
}
