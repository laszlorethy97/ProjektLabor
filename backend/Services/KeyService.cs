using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class KeyService
    {
        private readonly KeyManagementDbContext _context;

        public KeyService(KeyManagementDbContext context)
        {
            _context = context;
        }


    }
}