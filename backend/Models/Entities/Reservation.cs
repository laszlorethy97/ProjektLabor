using System.ComponentModel.DataAnnotations;
using KeyManagement.Api.Models.Enums;

namespace KeyManagement.Api.Models.Entities
{
    public class Reservation
    {
        
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int RequesterId { get; set; }
        public int? ApproverId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [StringLength(500)]
        public string? RejectionReason { get; set; } = null;
        public ReservationState State { get; set; } 
        public Room Room { get; set; } = null!;
        public User Requester { get; set; } = null!;
        public User? Approver { get; set; } 
        public ICollection<KeyTransaction> KeyTransactions { get; set; } = new List<KeyTransaction>();

    }
}