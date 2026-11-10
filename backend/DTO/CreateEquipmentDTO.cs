using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO
{
    public class CreateEquipmentDTO
    {
        [Required]
        [StringLength(100)]
        public string RoomName { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<string> EquipmentNames { get; set; } = new();
    }
}