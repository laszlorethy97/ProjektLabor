using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace KeyManagement.Api.Services
{
    public enum UserRegistrationError
    {
        None,
        EmailAlreadyRegistered,
        InvalidPinCode,
        AdministratorNotFound,
        InvalidRoles
    }

    public class UserService
    {
        private readonly KeyManagementDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserService(
            KeyManagementDbContext context,
            IConfiguration configuration,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        private async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        private bool IsPasswordValid(User user, string password)
        {
            var passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

            return passwordResult != PasswordVerificationResult.Failed;
        }

        private string BuildToken(User user)
        {
            var signingKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is missing.");
            var key = Encoding.UTF8.GetBytes(signingKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> { 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                };

                foreach (var userRole in user.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Type.ToLower()));
                }
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string?> LoginUser(LoginUserDTO loginUserDTO)
        {
            User? user = await FindUserByEmailAsync(loginUserDTO.Email);
            var successfulLogin = user is not null && IsPasswordValid(user, loginUserDTO.Password);
            await WriteLoginAuditAsync(user, loginUserDTO.Email, successfulLogin);

            if (!successfulLogin || user is null)
            {
                return null;
            }

            string token = BuildToken(user);
            return token;
        }

        private async Task WriteLoginAuditAsync(User? user, string attemptedEmail, bool successful)
        {
            var context = _httpContextAccessor?.HttpContext;
            _context.AuditLogs.Add(new AuditLog
            {
                EventType = AuditLogEventTypes.Authentication,
                ActorUserId = successful ? user?.Id : null,
                ActorEmail = successful ? Truncate(user?.Email, 256) : null,
                ActorRoles = successful
                    ? Truncate(string.Join(
                        ",",
                        user!.UserRoles.Select(userRole => userRole.Role.Type.ToLowerInvariant())), 256)
                    : null,
                Method = "LOGIN",
                Resource = "/api/User/login",
                Details = System.Text.Json.JsonSerializer.Serialize(new
                {
                    outcome = successful ? "success" : "failure",
                    attemptedEmail = successful ? null : Truncate(attemptedEmail, 256)
                }),
                CorrelationId = Truncate(context?.TraceIdentifier, 64),
                StatusCode = successful ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                OccurredAt = DateTime.UtcNow,
                IpAddress = Truncate(context?.Connection.RemoteIpAddress?.ToString(), 45),
                UserAgent = Truncate(context?.Request.Headers.UserAgent.ToString(), 512)
            });

            await _context.SaveChangesAsync(CancellationToken.None);
        }

        private static string? Truncate(string? value, int maximumLength)
        {
            return value is { Length: > 0 }
                ? value.Length <= maximumLength ? value : value[..maximumLength]
                : null;
        }

        public async Task<(bool Success, string? Error, UserRegistrationError ErrorCode)> RegisterUserAsync(
        RegisterUserDTO dto,
        int createdByUserId)
        {
            string email = dto.Email.Trim();
            if (await _context.Users.AnyAsync(u => u.Email.ToUpper() == email.ToUpper()))
            {
                return (false, "This email address is already registered.", UserRegistrationError.EmailAlreadyRegistered);
            }

            if (dto.PinCode.Length != 6 || !dto.PinCode.All(char.IsDigit))
            {
                return (false, "PIN code must contain exactly 6 digits.", UserRegistrationError.InvalidPinCode);
            }

            User? admin = await _context.Users.FindAsync(createdByUserId);

            if (admin == null)
            {
                return (false, "Administrator not found.", UserRegistrationError.AdministratorNotFound);
            }

            List<string> roleNames = dto.Roles
                .Select(r => r.Trim().ToLower())
                .Distinct()
                .ToList();

            List<Role> roles = await _context.Roles
                .Where(r => roleNames.Contains(r.Type.ToLower()))
                .ToListAsync();

            if (roles.Count != roleNames.Count)
            {
                return (false, "One or more roles do not exist.", UserRegistrationError.InvalidRoles);
            }

            User user = new User
            {
                Email = email,
                PinCode = dto.PinCode,
                CreatedById = admin.Id,
                CreatedBy = admin
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);

            foreach (Role role in roles)
            {
                UserRole userRole = new UserRole
                {
                    User = user,
                    Role = role,
                    CreatedBy = admin
                };

                _context.UserRoles.Add(userRole);
            }

            await _context.SaveChangesAsync();

            return (true, null, UserRegistrationError.None);
        }
    }
}