using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class KeyTransactionService
    {
        private readonly KeyManagementDbContext _context;

        public KeyTransactionService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<KeyTransaction>> GetAllAsync()
        {
            return await _context.KeyTransactions.ToListAsync();
        }
    }
}