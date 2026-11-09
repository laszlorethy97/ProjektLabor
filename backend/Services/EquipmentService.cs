using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
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

        public async Task<(bool Success, string? Error)> CreateEquipmentAsync(
            CreateEquipmentDTO dto, int userId)
        {
            Room? room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == dto.RoomId);

            if (room == null)
                return (false, "Room not found.");

            User? user = await _context.Users.FindAsync(userId);

            if (user == null)
                return (false, "User not found.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "Equipment name cannot be empty.");

            Equipment equipment = new Equipment
            {
                Name = dto.Name,
                Room = room,
                CreatedBy = user
            };

            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}