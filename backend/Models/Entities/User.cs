using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class User
    {
        
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;
        [StringLength(256)]
        public string PasswordHash{ get; set; } = string.Empty;
        [StringLength(6)]
        public string NeptunCode { get; set; } = string.Empty;
        [StringLength(15)]
        public string TelephoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<KeyTransaction> KeyTransactions { get; set; } = new List<KeyTransaction>();

    }
}