using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class KeyTransaction
    {
        
        public int Id { get; set; }
        public int KeyId { get; set; }
        public int? ReservationId { get; set; }
        public int HolderUserId { get; set; }
        public int IssuedByUserId { get; set; }
        public int? ReturnedToUserId { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnedAt { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; } = null;
        public Key Key { get; set; } = null!;
        public User HolderUser { get; set; } = null!;
        public User IssuedByUser { get; set; } = null!;
        public User? ReturnedToUser { get; set; }
        
    }
}