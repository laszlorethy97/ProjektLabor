using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO;

public class CreatePermissionDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int MasterKeyId { get; set; }
}
