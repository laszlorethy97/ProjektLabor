using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class Equipment
    {
        
        public int Id { get; set; }
        public int RoomId { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(50)]
        public string InventoryNumber { get; set; } = string.Empty;
        public bool IsWorking { get; set; } = true;
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}