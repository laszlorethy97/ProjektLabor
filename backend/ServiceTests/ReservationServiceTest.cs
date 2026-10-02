using Xunit;
using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.Services;

namespace KeyManagement.Api.ServiceTests
{
    public class ReservationServiceTest
    {
        private const int RoomId = 1;
        private const int OtherRoomId = 2;
        private const int UserId = 1;
        private const int ProjectorId = 1;
        private const int WhiteboardId = 2;
        private const int OtherRoomEquipmentId = 3;

        private  KeyManagementDbContext _GetDbContext()
        {
            var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new KeyManagementDbContext(options);
        }

        private async Task<KeyManagementDbContext> _GetSeededDbContext()
        {
            var context = _GetDbContext();
            var user = new User { Id = UserId, Email = "oktato@test.hu" };
            var building = new Building { Id = 1, Name = "Building 1" };
            var room = new Room { Id = RoomId, Name = "Room 1", Building = building };
            var otherRoom = new Room { Id = OtherRoomId, Name = "Room 2", Building = building };

            context.Equipments.Add(new Equipment { Id = ProjectorId, Name = "Projector", Room = room, CreatedBy = user });
            context.Equipments.Add(new Equipment { Id = WhiteboardId, Name = "Whiteboard", Room = room, CreatedBy = user });
            context.Equipments.Add(new Equipment { Id = OtherRoomEquipmentId, Name = "Speaker", Room = otherRoom, CreatedBy = user });
            await context.SaveChangesAsync();

            return context;
        }

        private static ReservationDTO _CreateReservationDTO(string startDate, params int[] equipmentIds)
        {
            return new ReservationDTO
            {
                RoomId = RoomId,
                StartDate = DateTime.Parse(startDate),
                EquipmentIds = equipmentIds.ToList()
            };
        }

        [Fact]
        public async Task CreateReservationAsync_IfTimeSlotIsFree_CreatesReservation()
        {
            using var context = await _GetSeededDbContext();
            var service = new ReservationService(context);

            await service.CreateReservationAsync(_CreateReservationDTO("2026-10-01 08:00", ProjectorId), UserId);

            var reservation = await context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .Include(r => r.Equipments)
                .SingleAsync();
            Assert.Equal(DateTime.Parse("2026-10-01 08:00"), reservation.StartTime);
            Assert.Equal(DateTime.Parse("2026-10-01 08:45"), reservation.EndTime);
            Assert.Equal(RoomId, reservation.Room.Id);
            Assert.Equal(UserId, reservation.User.Id);
            Assert.Equal(ProjectorId, Assert.Single(reservation.Equipments).Id);
        }

        [Theory]
        [InlineData("2026-10-01 07:00")]
        [InlineData("2026-10-01 09:00")]
        public async Task CreateReservationAsync_IfNeighbouringHourIsReserved_CreatesReservation(string startDate)
        {
            using var context = await _GetSeededDbContext();
            var service = new ReservationService(context);
            await service.CreateReservationAsync(_CreateReservationDTO("2026-10-01 08:00"), UserId);

            await service.CreateReservationAsync(_CreateReservationDTO(startDate), UserId);

            Assert.Equal(2, await context.Reservations.CountAsync());
        }

        [Fact]
        public async Task CreateReservationAsync_IfOtherRoomIsReserved_CreatesReservation()
        {
            using var context = await _GetSeededDbContext();
            var service = new ReservationService(context);
            var otherRoomReservation = _CreateReservationDTO("2026-10-01 08:00");
            otherRoomReservation.RoomId = OtherRoomId;
            await service.CreateReservationAsync(otherRoomReservation, UserId);

            await service.CreateReservationAsync(_CreateReservationDTO("2026-10-01 08:00"), UserId);

            Assert.Equal(2, await context.Reservations.CountAsync());
        }

        [Fact]
        public async Task CreateReservationAsync_IfTimeSlotIsReserved_ThrowsConflict()
        {
            using var context = await _GetSeededDbContext();
            var service = new ReservationService(context);
            await service.CreateReservationAsync(_CreateReservationDTO("2026-10-01 08:00"), UserId);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.CreateReservationAsync(_CreateReservationDTO("2026-10-01 08:00"), UserId);
            });
            Assert.Equal(1, await context.Reservations.CountAsync());
        }

        [Theory]
        [InlineData("2026-10-01 07:30", "2026-10-01 08:15")]
        [InlineData("2026-10-01 08:30", "2026-10-01 10:00")]
        [InlineData("2026-10-01 08:10", "2026-10-01 08:20")]
        [InlineData("2026-09-30 00:00", "2026-10-02 00:00")]
        public async Task CreateReservationAsync_IfRoomIsUnderMaintenance_ThrowsConflict(string maintenanceStart, string maintenanceEnd)
        {
            using var context = await _GetSeededDbContext();
            context.Maintenances.Add(new Maintenance
            {
                StartTime = DateTime.Parse(maintenanceStart),
                EndTime = DateTime.Parse(maintenanceEnd),
                Room = await context.Rooms.SingleAsync(r => r.Id == RoomId),
                User = await context.Users.SingleAsync(u => u.Id == UserId)
            });
            await context.SaveChangesAsync();
            var service = new ReservationService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.CreateReservationAsync(_CreateReservationDTO("2026-10-01 08:00"), UserId);
            });
            Assert.Empty(context.Reservations);
        }

        [Fact]
        public async Task CreateReservationAsync_IfEquipmentBelongsToOtherRoom_ThrowsError()
        {
            using var context = await _GetSeededDbContext();
            var service = new ReservationService(context);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await service.CreateReservationAsync(_CreateReservationDTO("2026-10-01 08:00", OtherRoomEquipmentId), UserId);
            });
            Assert.Empty(context.Reservations);
        }

        [Fact]
        public async Task CreateReservationAsync_IfRoomDoesNotExist_ThrowsError()
        {
            using var context = await _GetSeededDbContext();
            var service = new ReservationService(context);
            var reservationDTO = _CreateReservationDTO("2026-10-01 08:00");
            reservationDTO.RoomId = 99;

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.CreateReservationAsync(reservationDTO, UserId);
            });
        }
    }
}
