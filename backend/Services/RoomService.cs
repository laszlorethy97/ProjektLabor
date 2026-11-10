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

        public async Task<RoomScheduleDTO?> GetRoomScheduleAsync(int roomId)
        {
            Room? room = await _context.Rooms
                .Include(r => r.Reservations)
                .Include(r => r.Maintenances)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
            {
                return null;
            }

            return new RoomScheduleDTO
            {
                Reservations = room.Reservations
                    .Select(r => new TimeIntervalDTO
                    {
                        StartTime = r.StartTime,
                        EndTime = r.EndTime
                    })
                    .ToList(),

                Maintenances = room.Maintenances
                    .Select(m => new TimeIntervalDTO
                    {
                        StartTime = m.StartTime,
                        EndTime = m.EndTime
                    })
                    .ToList()
            };
        }

        public async Task<(bool Success, string? Error)> CreateRoomAsync(
            CreateRoomDTO dto, int userId)
        {
            string roomName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(roomName))
                return (false, "Room name cannot be empty.");

            bool roomExists = await _context.Rooms
                .AnyAsync(r => r.Name.ToUpper() == roomName.ToUpper());

            if (roomExists)
                return (false, "A room with this name already exists.");

            string requestedBuildingName = dto.BuildingName.Trim();
            if (string.IsNullOrWhiteSpace(requestedBuildingName))
                return (false, "Building name cannot be empty.");

            string normalizedBuildingName = requestedBuildingName.ToUpperInvariant();
            List<Building> matchingBuildings = await _context.Buildings
                .Where(b => b.Name.ToUpper() == normalizedBuildingName)
                .Take(2)
                .ToListAsync();

            if (matchingBuildings.Count == 0)
                return (false, "Building not found.");

            if (matchingBuildings.Count > 1)
                return (false, "A building with this name already exists.");

            Building building = matchingBuildings[0];
            User? user = await _context.Users.FindAsync(userId);

            if (user == null)
                return (false, "User not found.");

            if (dto.Capacity <= 0)
                return (false, "Capacity must be greater than zero.");

            Room room = new Room
            {
                Name = roomName,
                Capacity = dto.Capacity,
                Building = building
            };

            foreach (string equipmentName in dto.Equipments)
            {
                if (string.IsNullOrWhiteSpace(equipmentName))
                    return (false, "Equipment name cannot be empty.");

                room.Equipments.Add(new Equipment
                {
                    Name = equipmentName,
                    CreatedBy = user
                });
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}