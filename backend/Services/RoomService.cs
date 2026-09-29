using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.DTO;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class RoomService
    {
        private readonly KeyManagementDbContext _context;

        public RoomService(KeyManagementDbContext context)
        {
            _context = context;
        }


        public async Task<List<RoomDTO>> GetRoomsByBuildingAsync()
        {
            return await _context.Rooms
                .Include(r => r.Building)
                .OrderBy(r => r.Building.Name)
                .ThenBy(r => r.Name)
                .Select(r => new RoomDTO
                {
                    RoomId = r.Id,
                    RoomName = r.Name,
                    Capacity = r.Capacity,
                    BuildingName = r.Building.Name
                })
                .ToListAsync();
        }
    }
}