

namespace KeyManagement.Api.Models.Entities
{
    public class UserRole
    {
        
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User CreatedBy { get; set; } = null!;
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
       
    }
}