using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class UserRoleService
    {
        private readonly KeyManagementDbContext _context;

        public UserRoleService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserRole>> GetAllAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }
    }
}