using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class KeyTransaction
    {
        
        public int Id { get; set; }
        [StringLength(6)]
        public string PinCode { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public User StartedByUser { get; set; } = null!;
        public User? EndedByUser { get; set; }
        public User User { get; set; } = null!;
        public Key Key { get; set; } = null!;
    }
}