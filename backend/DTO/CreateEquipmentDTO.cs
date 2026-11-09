using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO
{
    public class CreateEquipmentDTO
    {
        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}