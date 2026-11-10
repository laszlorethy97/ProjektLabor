using Xunit;
using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task GetAllAsync_IfUsersExist_GetAll()
        {
            using var context = _GetDbContext();
            context.Users.Add(new User { Id = 1, Email = "john.doe@example.com", PasswordHash = "password123" });
            context.Users.Add(new User { Id = 2, Email = "jane.doe@example.com", PasswordHash = "password456" });
            await context.SaveChangesAsync();
            
            var service = new UserService(context);
            var result = await service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("john.doe@example.com", result[0].Email);
            Assert.Equal("jane.doe@example.com", result[1].Email);
        }

        [Fact]
        public async Task LoginUser_IfUserIsValid_ReturnsToken()
        {
            using var context = _GetDbContext();
            var service = new UserService(context);

            var user1 = new User
            {
                Id = 1,
                Email = "john.doe@example.com"
            };

            var passwordHasher = new PasswordHasher<User>();
            user1.PasswordHash = passwordHasher.HashPassword(user1, "password123");
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

           var user1 = new User
            {
                Id = 1,
                Email = "john.doe@example.com"
            };

            var passwordHasher = new PasswordHasher<User>();
            user1.PasswordHash = passwordHasher.HashPassword(user1, "password123");
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

        [Fact]
        public async Task RegisterUserAsync_DuplicateEmailIgnoringCase_ReturnsConflictError()
        {
            using var context = _GetDbContext();
            context.Users.AddRange(
                new User { Id = 1, Email = "admin@example.com" },
                new User { Id = 2, Email = "existing@example.com" });
            await context.SaveChangesAsync();

            var controller = new UserController(new UserService(context))
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.NameIdentifier, "1")
                        ], "test"))
                    }
                }
            };

            var result = await controller.Register(new RegisterUserDTO
            {
                Email = " EXISTING@example.com ",
                PinCode = "123456",
                Password = "secure-password",
                Roles = ["oktato"]
            });

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);
        }

        [Fact]
        public async Task RegisterUserAsync_ValidRequest_ReturnsCreated()
        {
            using var context = _GetDbContext();
            context.Users.Add(new User { Id = 1, Email = "admin@example.com" });
            context.Roles.Add(new Role { Id = 1, Type = "oktato" });
            await context.SaveChangesAsync();

            var controller = new UserController(new UserService(context))
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.NameIdentifier, "1")
                        ], "test"))
                    }
                }
            };

            var result = await controller.Register(new RegisterUserDTO
            {
                Email = " New.User@example.com ",
                PinCode = "123456",
                Password = "secure-password",
                Roles = ["oktato"]
            });

            var created = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
            Assert.Contains(context.Users, user => user.Email == "New.User@example.com");
        }

    }
}