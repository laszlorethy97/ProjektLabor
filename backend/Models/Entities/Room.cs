using System.ComponentModel.DataAnnotations;
using KeyManagement.Api.Models.Enums;

namespace KeyManagement.Api.Models.Entities
{
    public class Room
    {
        
        public int Id { get; set; }
        public int BuildingId { get; set; }
        [StringLength(5)]
        public string DoorNumber { get; set; } = string.Empty;
        [StringLength(100)]
        public string Name{ get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = true;
        
        public RoomType RoomType { get; set; }
        public Building Building { get; set; } = null!;
    }
}