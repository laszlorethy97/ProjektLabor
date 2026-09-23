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
    public class UserService
    {
        private readonly KeyManagementDbContext _context;

        public UserService(KeyManagementDbContext context)
        {
            _context = context;
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

        private static bool IsPasswordValid(User user, string password)
        {
            if (password == user.PasswordHash)
            {
                return true;
            }

            var passwordHasher = new PasswordHasher<User>();
            var passwordResult = passwordHasher.VerifyHashedPassword(
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

    }
}