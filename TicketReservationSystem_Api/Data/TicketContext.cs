using Microsoft.EntityFrameworkCore;
using TicketReservationSystem.Api.Models;

namespace TicketReservationSystem.Api.Data
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }

}
