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

            _context.MasterKeyTransactions = null;
            
            _context.MasterKeys.Add(newMasterKey);
            await _context.SaveChangesAsync();

            return newMasterKey;
        }
    }
}
