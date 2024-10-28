using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketReservationSystem.Api.Data;
using TicketReservationSystem.Api.Models;

namespace TicketReservationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly TicketContext _context;

        public EventController(TicketContext context)
        {
            _context = context;
        }

        // Existing method to get bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetEvents()
        {
            var events = await _context.Bookings.ToListAsync();
            return Ok(events);
        }

        // New method to get unique venues from the Event table
        [HttpGet("unique-venues")]
        public async Task<ActionResult<IEnumerable<string>>> GetUniqueVenues()
        {
            var venues = await _context.Events
                .Select(e => e.Venue) // Assuming 'Venue' is a property in your Event model
                .Distinct() // Get unique venues
                .ToListAsync();

            return venues;
        }
    }
}
