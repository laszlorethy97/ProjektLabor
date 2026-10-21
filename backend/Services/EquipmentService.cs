using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;

using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class EquipmentService
    {
        private readonly KeyManagementDbContext _context;

        public EquipmentService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<EquipmentDTO>> GetEquipmentsByRoomId(int roomId)
        {
            return await _context.Equipments
                .Where(e => e.Room.Id == roomId)
                .Select(e => new EquipmentDTO
                {
                    Id = e.Id,
                    Name = e.Name
                })
                .ToListAsync();
        }
    }
}