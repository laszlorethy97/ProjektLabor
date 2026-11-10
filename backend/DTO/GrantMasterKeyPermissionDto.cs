using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO;

public class GrantMasterKeyPermissionDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public int MasterKeyId { get; set; }
}