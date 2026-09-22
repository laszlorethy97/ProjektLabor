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

        public async Task<List<MasterKey>> GetAllAsync()
        {
            return await _context.MasterKeys.ToListAsync();
        }
    }
}