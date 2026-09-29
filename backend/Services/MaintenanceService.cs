using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class MaintenanceService
    {
        private readonly KeyManagementDbContext _context;

        public MaintenanceService(KeyManagementDbContext context)
        {
            _context = context;
        }


    }
}