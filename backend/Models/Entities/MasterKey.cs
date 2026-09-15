namespace KeyManagement.Api.Models.Entities
{
    public class MasterKey
    {
        
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User CreatedBy { get; set; } = null!;
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<MasterKeyTransaction> MasterKeyTransactions { get; set; } = new List<MasterKeyTransaction>();
    }
}