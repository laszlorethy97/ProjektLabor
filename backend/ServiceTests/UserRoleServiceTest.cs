using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests
{
    public class UserRoleServiceTest
    {
        private static KeyManagementDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new KeyManagementDbContext(options);
        }

        [Fact]
        public async Task CreateUserRoleAsyncFindsUserByEmailIgnoringCaseAndWhitespace()
        {
            using var context = CreateDbContext();
            var admin = new User { Id = 1, Email = "admin@example.com" };
            var user = new User { Id = 2, Email = "user@example.com" };
            var role = new Role { Id = 1, Type = "Admin" };
            context.Users.AddRange(admin, user);
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var service = new UserRoleService(context);
            var result = await service.CreateUserRoleAsync(" USER@EXAMPLE.COM ", [role.Id], admin.Id);

            var assignedRole = Assert.Single(result);
            Assert.Equal(user.Id, assignedRole.User.Id);
            Assert.Equal(role.Id, assignedRole.Role.Id);
            Assert.Equal(admin.Id, assignedRole.CreatedBy.Id);
        }

        [Fact]
        public async Task CreateUserRoleAsyncThrowsWhenEmailDoesNotMatchAUser()
        {
            using var context = CreateDbContext();
            context.Users.Add(new User { Id = 1, Email = "admin@example.com" });
            await context.SaveChangesAsync();

            var service = new UserRoleService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.CreateUserRoleAsync("missing@example.com", [1], 1));
        }
    }
}
