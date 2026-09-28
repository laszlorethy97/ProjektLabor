using Xunit;
using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.Services;

namespace KeyManagement.Api.ServiceTests
{
    public class BuildingServiceTest
    {
        private  KeyManagementDbContext _GetDbContext()
        {
            var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new KeyManagementDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_IfBuildingsExist_GetAll()
        {
            using var context = _GetDbContext();
            context.Buildings.Add(new Building { Id = 1, Name = "Building 1" });
            context.Buildings.Add(new Building { Id = 2, Name = "Building 2" });
            await context.SaveChangesAsync();
            
            var service = new BuildingService(context);
            var result = await service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Building 1", result[0].Name);
            Assert.Equal("Building 2", result[1].Name);
        }

        [Theory]
        [InlineData(1, "Building 1")]
        [InlineData(2, "Building 2")]
        public async Task GetAllAsync_IfBuildingsExist_GetOne(int id,string expectedName)
        {
            using var context = _GetDbContext();
            context.Buildings.Add(new Building { Id = 1, Name = "Building 1" });
            context.Buildings.Add(new Building { Id = 2, Name = "Building 2" });
            await context.SaveChangesAsync();
            
            var service = new BuildingService(context);
            
            var result = await service.GetByIdAsync(id); 

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(expectedName, result.Name);
        }
    }
}