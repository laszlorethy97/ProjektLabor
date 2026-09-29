using Xunit;
using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.Services;
using Microsoft.IdentityModel.Tokens;

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

    }
}