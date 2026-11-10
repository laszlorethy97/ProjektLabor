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
            string requestedRoomName = dto.RoomName.Trim();
            if (string.IsNullOrWhiteSpace(requestedRoomName))
                return (false, "Room name cannot be empty.");

            string normalizedRoomName = requestedRoomName.ToUpperInvariant();
            List<Room> matchingRooms = await _context.Rooms
                .Where(r => r.Name.ToUpper() == normalizedRoomName)
                .Take(2)
                .ToListAsync();

            if (matchingRooms.Count == 0)
                return (false, "Room not found.");

            if (matchingRooms.Count > 1)
                return (false, "Multiple rooms found with this name.");

            Room room = matchingRooms[0];
            User? user = await _context.Users.FindAsync(userId);

            if (user == null)
                return (false, "User not found.");

            List<string> equipmentNames = dto.EquipmentNames
                .Select(name => name.Trim())
                .ToList();

            if (equipmentNames.Count == 0)
                return (false, "At least one equipment name is required.");

            if (equipmentNames.Any(string.IsNullOrWhiteSpace))
                return (false, "Equipment name cannot be empty.");

            if (equipmentNames.Any(name => name.Length > 100))
                return (false, "Equipment names cannot exceed 100 characters.");

            List<Equipment> equipment = equipmentNames
                .Select(name => new Equipment
                {
                    Name = name,
                    Room = room,
                    CreatedBy = user
                })
                .ToList();

            await _context.Equipments.AddRangeAsync(equipment);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}