using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.DTO;

public class CreatePermissionDTO
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int MasterKeyId { get; set; }
}
