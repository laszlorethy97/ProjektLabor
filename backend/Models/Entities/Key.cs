using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class Key
    {
        
        public int Id { get; set; }
        [StringLength(25)]
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User CreatedBy { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public ICollection<KeyTransaction> KeyTransactions { get; set; } = new List<KeyTransaction>();
    }
}