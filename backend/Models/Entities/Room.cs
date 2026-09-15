using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class Room
    {
        
        public int Id { get; set; }
        [StringLength(100)]
        public string Name{ get; set; } = string.Empty;
        public int Capacity { get; set; }
        public Building Building { get; set; } = null!;
        public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Key> Keys { get; set; } = new List<Key>();
        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    }
}