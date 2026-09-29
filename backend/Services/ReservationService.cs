using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class ReservationService
    {
        private readonly KeyManagementDbContext _context;

        public ReservationService(KeyManagementDbContext context)
        {
            _context = context;
        }


    }
}