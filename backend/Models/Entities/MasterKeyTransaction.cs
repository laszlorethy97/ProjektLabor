using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class MasterKeyTransaction
    {
        public int Id { get; set; }
        [StringLength(6)]
        public string PinCode { get; set; } = string.Empty;
         public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public User StartedByUser { get; set; } = null!;
        public User? EndedByUser { get; set; }
        public User ApprovedByUser { get; set; } = null!;
        public User User { get; set; } = null!;
        public MasterKey MasterKey { get; set; } = null!;
    }
}
