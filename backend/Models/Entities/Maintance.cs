namespace KeyManagement.Api.Models.Entities
{
    public class Maintance
    {
        
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Room Room { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}