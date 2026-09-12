using System.ComponentModel.DataAnnotations;
using KeyManagement.Api.Models.Enums;

namespace KeyManagement.Api.Models.Entities
{
    public class Key
    {
        
        public int Id { get; set; }
        public int? RoomId { get; set; }
        [StringLength(25)]
        public string Tag { get; set; } = string.Empty;
        public KeyType KeyType { get; set; }
        public KeyState KeyState { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Room Room { get; set; } = null!;
    }
}