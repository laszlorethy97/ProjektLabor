using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class MasterKeyService
    {
        private readonly KeyManagementDbContext _context;

        public MasterKeyService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<MasterKey>> GetMasterKeysAsync()
        {
            return await _context.MasterKeys.ToListAsync();
        }

        public async Task<MasterKey> CreateMasterKeyAsync(int createdByUserId)
        {
            var createdByUser = await _context.Users.FindAsync(createdByUserId);
            
            if (createdByUser == null)
            {
                throw new KeyNotFoundException("The Creator User was not found.");
            }

            var newMasterKey = new MasterKey
            {
                CreatedBy = createdByUser
            };

            _context.MasterKeys.Add(newMasterKey);
            await _context.SaveChangesAsync();

            return newMasterKey;
        }

        public async Task<Permission> GrantMasterKeyPermissionAsync(int userId, int masterKeyId, int createdByUserId)
        {
            
            var user = await _context.Users.FindAsync(userId);
            var masterKey = await _context.MasterKeys.FindAsync(masterKeyId);
            var createdByUser = await _context.Users.FindAsync(createdByUserId);

            if (user == null || masterKey == null || createdByUser == null)
            {
                throw new KeyNotFoundException("User, MasterKey, or Creator User not found.");
            }

            var permissionExists = await _context.Permissions
                .AnyAsync(p => p.User.Id == userId && p.MasterKey.Id == masterKeyId);

            if (permissionExists!= null)
            {
                throw new InvalidOperationException("This user already has permission for this master key.");
            }

            var newPermission = new Permission
            {
                User = user,
                MasterKey = masterKey,
                CreatedBy = createdByUser
            };

            _context.Permissions.Add(newPermission);
            await _context.SaveChangesAsync();

            return newPermission;
        }
    }
}
