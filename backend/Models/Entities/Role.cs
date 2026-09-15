using System.ComponentModel.DataAnnotations;

namespace KeyManagement.Api.Models.Entities
{
    public class Role
    {
        
        public int Id { get; set; }
        [StringLength(50)]
        public string Type{ get; set; }= string.Empty;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
       

    }
}