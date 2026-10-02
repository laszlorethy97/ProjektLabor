using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class ReservationService
    {
        private static readonly TimeSpan ReservationLength = TimeSpan.FromMinutes(45);

        private readonly KeyManagementDbContext _context;

        public ReservationService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<Reservation> CreateReservationAsync(ReservationDTO reservationDTO, int userId)
        {
            var room = await FindRoomAsync(reservationDTO.RoomId);
            var user = await FindUserAsync(userId);
            var equipments = await FindEquipmentsAsync(room.Id, reservationDTO.EquipmentIds);

            var startTime = reservationDTO.StartDate;
            var endTime = startTime.Add(ReservationLength);
            await EnsureRoomIsFreeAsync(room.Id, startTime, endTime);

            var reservation = new Reservation
            {
                StartTime = startTime,
                EndTime = endTime,
                Room = room,
                User = user,
                Equipments = equipments
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return reservation;
        }

        private async Task<Room> FindRoomAsync(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found.");
            }

            return room;
        }

        private async Task<User> FindUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user;
        }

        private async Task<List<Equipment>> FindEquipmentsAsync(int roomId, List<int> equipmentIds)
        {
            var requestedIds = equipmentIds.Distinct().ToList();

            var equipments = await _context.Equipments
                .Where(e => e.Room.Id == roomId && requestedIds.Contains(e.Id))
                .ToListAsync();

            if (equipments.Count != requestedIds.Count)
            {
                throw new ArgumentException("One or more equipments do not belong to the room.");
            }

            return equipments;
        }

        private async Task EnsureRoomIsFreeAsync(int roomId, DateTime startTime, DateTime endTime)
        {
            bool isReserved = await _context.Reservations
                .AnyAsync(r => r.Room.Id == roomId && r.StartTime < endTime && r.EndTime > startTime);

            bool isUnderMaintenance = await _context.Maintenances
                .AnyAsync(m => m.Room.Id == roomId && m.StartTime < endTime && m.EndTime > startTime);

            if (isReserved || isUnderMaintenance)
            {
                throw new InvalidOperationException("The room is not available in this time slot.");
            }
        }
    }
}
