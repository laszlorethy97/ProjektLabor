using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class UserRole
    {
        
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
       
    }
}