using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO;

public class AssignRolesDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public List<int> RoleIds { get; set; } = new List<int>();
}