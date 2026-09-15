using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class Building
    {
        
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}