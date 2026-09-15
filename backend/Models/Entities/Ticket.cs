namespace KeyManagement.Api.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? CreatedBy { get; set; }
    }
}