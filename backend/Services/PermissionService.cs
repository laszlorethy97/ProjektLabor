using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class PermissionService
    {
        private readonly KeyManagementDbContext _context;

        public PermissionService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Permission> CreatePermissionAsync(CreatePermissionDTO dto, int createdByUserId)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var masterKey = await _context.MasterKeys.FindAsync(dto.MasterKeyId);
            if (masterKey == null)
            {
                throw new KeyNotFoundException("Master key not found.");
            }

            var createdBy = await _context.Users.FindAsync(createdByUserId);
            if (createdBy == null)
            {
                throw new KeyNotFoundException("Admin user not found.");
            }

            bool permissionExists = await _context.Permissions
                .AnyAsync(permission =>
                    permission.User.Id == user.Id &&
                    permission.MasterKey.Id == masterKey.Id);
            if (permissionExists)
            {
                throw new InvalidOperationException("User already has permission to use this master key.");
            }

            var permission = new Permission
            {
                User = user,
                MasterKey = masterKey,
                CreatedBy = createdBy
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return permission;
        }
    }
}
