using Xunit;
using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using KeyManagement.Api.DTO;

namespace KeyManagement.Api.ServiceTests
{
    public class UserServiceTest
    {
        private  KeyManagementDbContext _GetDbContext()
        {
            var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new KeyManagementDbContext(options);
        }

        [Fact]
        public async Task LoginUser_IfUserIsValid_ReturnsToken()
        {
            using var context = _GetDbContext();
            var service = new UserService(context);

            var user1 = new User { Id = 1, Email = "john.doe@example.com", PasswordHash = "password123" };
            var role1 = new Role { Id = 1, Type = "Admin" };
            var userRole = new UserRole { Id = 1, User=user1, Role=role1 };

            context.Users.Add(user1);
            context.Roles.Add(role1);
            context.UserRoles.Add(userRole);

            await context.SaveChangesAsync();

            var dto = new LoginUserDTO { Email = "john.doe@example.com", Password = "password123" };
            var token = await service.LoginUser(dto);
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

       [Fact]
        public async Task LoginUser_IfPasswordIsInvalid_ReturnsNull()
        {
            using var context = _GetDbContext();
            var service = new UserService(context);

            var hasher = new PasswordHasher<User>();
            var dummyUser = new User();
            var hashedPassword = hasher.HashPassword(dummyUser, "password123");

            var user1 = new User { Id = 1, Email = "john.doe@example.com", PasswordHash = hashedPassword };
            var role1 = new Role { Id = 1, Type = "Admin" };
            var userRole = new UserRole { Id = 1, User=user1, Role=role1 };

            context.Users.Add(user1);
            context.Roles.Add(role1);
            context.UserRoles.Add(userRole);

            await context.SaveChangesAsync();

            var dto = new LoginUserDTO { Email = "john.doe@example.com", Password = "wrongpassword" };
            var token = await service.LoginUser(dto);
            Assert.Null(token);
        }

        [Fact]
        public async Task LoginUser_IfEmailIsInvalid_ReturnsNull()
        {
            using var context = _GetDbContext();
            var service = new UserService(context);

            var user1 = new User { Id = 1, Email = "john.doe@example.com", PasswordHash = "password123" };
            var role1 = new Role { Id = 1, Type = "Admin" };
            var userRole = new UserRole { Id = 1, User=user1, Role=role1 };

            context.Users.Add(user1);
            context.Roles.Add(role1);
            context.UserRoles.Add(userRole);

            await context.SaveChangesAsync();

            var dto = new LoginUserDTO { Email = "invalid@example.com", Password = "password123" };
            var token = await service.LoginUser(dto);
            Assert.Null(token);
        }

    }
}