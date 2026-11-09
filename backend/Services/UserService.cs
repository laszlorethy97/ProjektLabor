using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        public UserService(KeyManagementDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
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
            var key = Encoding.ASCII.GetBytes("ezEgyNagyonTitkosKulcs123!Megerkezo");
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> { 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
            if (user == null || !IsPasswordValid(user, loginUserDTO.Password))
            {
                return null;
            }

            string token = BuildToken(user);
            return token;
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