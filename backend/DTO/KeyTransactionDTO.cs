using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO
{
    public class KeyTransactionDTO
    {
        public string Email { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;

        [Required]
        [StringLength(25)]
        public string KeyCode { get; set; } = string.Empty;
    }
}