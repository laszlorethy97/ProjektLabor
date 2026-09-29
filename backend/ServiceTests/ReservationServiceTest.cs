using Xunit;
using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.Services;
using Microsoft.IdentityModel.Tokens;

namespace KeyManagement.Api.ServiceTests
{
    public class ReservationServiceTest
    {
        private  KeyManagementDbContext _GetDbContext()
        {
            var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new KeyManagementDbContext(options);
        }

        [Theory]
        [InlineData("2026-10-01 08:00", "2026-10-01 09:00")]
        [InlineData("2026-10-01 08:00", "2026-10-01 08:30")]
        [InlineData("2026-10-01 08:00", "2026-10-01 08:50")]
        public async Task CreateReservationAsync_IfTimeSlotsAreInvalid_ReturnsError(string startTimeStr, string endTimeStr)
        {
            using var context = _GetDbContext();
            var service = new ReservationService(context);

            DateTime startTime = DateTime.Parse(startTimeStr);
            DateTime endTime = DateTime.Parse(endTimeStr);

            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                await service.CreateReservationAsync(1, startTime, endTime, 1);
            });
        }
        
        

    }
}