
using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO
{
    public class MasterKeyTransactionDTO
    {
        public string Email { get; set; } = string.Empty;

        [StringLength(6)]
        public string PinCode { get; set; } = string.Empty;

        public int MasterKeyId { get; set; }
    }
}
