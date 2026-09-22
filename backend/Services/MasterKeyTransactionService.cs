using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class MasterKeyTransactionService
    {
        private readonly KeyManagementDbContext _context;

        public MasterKeyTransactionService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<MasterKeyTransaction>> GetAllAsync()
        {
            return await _context.MasterKeyTransactions.ToListAsync();
        }
    }
}