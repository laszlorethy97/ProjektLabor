using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class EquipmentService
    {
        private readonly KeyManagementDbContext _context;

        public EquipmentService(KeyManagementDbContext context)
        {
            _context = context;
        }


    }
}