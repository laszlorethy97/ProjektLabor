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
        
        public async Task<List<UserRole>> GetUserRolesAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<UserRole?> GetUserRoleByIdAsync(int id)
        {
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<User> GetCreatedByUser(int userRoleId)
        {
            var CreatedByUserId = await _context.UserRoles
                .Where(ur => ur.Id == userRoleId)
                .Select(ur => ur.CreatedBy)
                .FirstOrDefaultAsync();

            if (CreatedByUserId  == null)
            {
                throw new KeyNotFoundException($"The CreatorUser Id for this UserRole is not found.");
            }

            return CreatedByUserId ;
        }
    
        public async Task<List<UserRole>> CreateUserRoleAsync(int userId, List<int> roleIds, int createdByUserId)
        {

            var user = await _context.Users.FindAsync(userId);
            var createdByUser = await _context.Users.FindAsync(createdByUserId);

            if (user == null || createdByUser == null)
            {
                throw new KeyNotFoundException("User or CreatedByUser not found.");
            }

            var givenRoles = await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

    
            if (givenRoles.Count != roleIds.Count)
            {
                throw new KeyNotFoundException("One or more specified Roles do not exist.");
            }

            var existingUserRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.User.Id == userId)
                .ToListAsync();

            var existingRoleIds = existingUserRoles.Select(ur => ur.Role.Id).ToList();
   
            var userRolesToRemove = existingUserRoles
                .Where(ur => !roleIds.Contains(ur.Role.Id))
                .ToList();

            if (userRolesToRemove.Count > 0)
            {
                _context.UserRoles.RemoveRange(userRolesToRemove);
            }

            var rolesToAdd = givenRoles
                .Where(r => !existingRoleIds.Contains(r.Id))
                .ToList();

            var newUserRoles = new List<UserRole>();

            foreach (var role in rolesToAdd)
            {
                var newUserRole = new UserRole
                {
                    User = user,          
                    Role = role,
                    CreatedBy = createdByUser
                };
                newUserRoles.Add(newUserRole);
            }

            if (newUserRoles.Count > 0)
            {
                await _context.UserRoles.AddRangeAsync(newUserRoles);
            }

            await _context.SaveChangesAsync();
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.User.Id == userId)
                .ToListAsync();
        }
    }
}