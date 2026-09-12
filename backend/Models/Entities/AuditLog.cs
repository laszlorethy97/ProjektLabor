using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class AuditLog
    {
        
        public int Id { get; set; }
        public int? UserId { get; set; }
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public User? User { get; set; }
    }
}