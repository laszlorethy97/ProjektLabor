using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class BuildingService
    {
        private readonly KeyManagementDbContext _context;

        public BuildingService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<Building>> GetAllAsync()
        {
            return await _context.Buildings.ToListAsync();
        }
    }
}