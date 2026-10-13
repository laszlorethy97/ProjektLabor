using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class TicketService
    {
        private readonly KeyManagementDbContext _context;

        public TicketService(KeyManagementDbContext context)
        {
            _context = context;
        }

        
        public async Task<Ticket> CreateTicketAsync(
            string description,
            int createdByUserId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == createdByUserId);

            if (user == null)
            {
                throw new KeyNotFoundException("A felhasználó nem található.");
            }

            var ticket = new Ticket
            {
                Description = description,
                CreatedBy = user
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

    }
}