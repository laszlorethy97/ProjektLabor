using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class Building
    {
        
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}