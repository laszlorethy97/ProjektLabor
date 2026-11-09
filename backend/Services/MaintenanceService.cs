using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.DTO;

namespace KeyManagement.Api.Services
{
    public class MaintenanceService
    {
        private readonly KeyManagementDbContext _context;

        public MaintenanceService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string? Error)> CreateMaintenanceAsync(
            CreateMaintenanceDTO dto,
            int userId)
        {
            if (dto.StartTime <= DateTime.UtcNow)
            {
                return (false, "Maintenance must start in the future.");
            }

            if (dto.StartTime >= dto.EndTime)
            {
                return (false, "Start time must be earlier than end time.");
            }

            Room? room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == dto.RoomId);

            if (room == null)
            {
                return (false, "Room not found.");
            }

            User? user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return (false, "User not found.");
            }

            bool reservationConflict = await _context.Reservations
                .AnyAsync(r =>
                    r.Room.Id == dto.RoomId &&
                    dto.StartTime < r.EndTime &&
                    dto.EndTime > r.StartTime);

            bool maintenanceConflict = await _context.Maintenances
                .AnyAsync(m =>
                    m.Room.Id == dto.RoomId &&
                    dto.StartTime < m.EndTime &&
                    dto.EndTime > m.StartTime);

            if (reservationConflict || maintenanceConflict)
            {
                return (false, "The selected time interval is already occupied.");
            }

            Maintenance maintenance = new Maintenance
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Room = room,
                User = user
            };

            _context.Maintenances.Add(maintenance);

            await _context.SaveChangesAsync();

            return (true, null);
        }

    }
}