using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO
{
    public class CreateRoomDTO
    {
        [Required]
        [StringLength(100)]
        public string BuildingName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public List<string> Equipments { get; set; } = new();
    }
}