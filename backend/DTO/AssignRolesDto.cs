namespace KeyManagement.Api.DTO;

public class AssignRolesDto
{
    public int UserId { get; set; }
    public List<int> RoleIds { get; set; } = new List<int>();
}