using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO
{
    public class RegisterUserDTO
    {
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, RegularExpression(@"^\d{6}$")]
        public string PinCode { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required, MinLength(1)]
        public List<string> Roles { get; set; } = [];
    }
}
