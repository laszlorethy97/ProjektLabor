using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class KeyService
    {
        private readonly KeyManagementDbContext _context;

        public KeyService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<Key>> GetKeysAsync()
        {
            return await _context.Keys.ToListAsync();
        }

        public async Task<Key> CreateKeyAsync(string code, string roomName, int createdByUserId)
        {
            code = code.Trim();
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Name == roomName);

            if (room == null)
            {
                throw new KeyNotFoundException($"The room was not found.");
            }

            var keyExists = await _context.Keys
                .AnyAsync(k => k.Code.ToUpper() == code.ToUpper());

            if (keyExists)
            {
                throw new InvalidOperationException("A key with this code already exists.");
            }

            var createdByUser = await _context.Users.FindAsync(createdByUserId);
            if (createdByUser == null)
            {
                throw new KeyNotFoundException("The Creator User was not found.");
            }

            var newKey = new Key
            {
                Code = code,
                Room = room,
                CreatedBy = createdByUser
            };

            _context.Keys.Add(newKey);
            await _context.SaveChangesAsync();

            return newKey;
        }
    }
}
