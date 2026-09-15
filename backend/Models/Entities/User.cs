using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class User
    {
        
        public int Id { get; set; }
        
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;
        [StringLength(256)]
        public string PasswordHash{ get; set; } = string.Empty;
        [StringLength(6)]
        public string PinCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<MasterKeyTransaction> MasterKeyTransactions { get; set; } = new List<MasterKeyTransaction>();
        public ICollection<KeyTransaction> KeyTransactions { get; set; } = new List<KeyTransaction>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}