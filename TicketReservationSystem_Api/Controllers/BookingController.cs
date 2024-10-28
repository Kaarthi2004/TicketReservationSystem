using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketReservationSystem.Api.Data;
using TicketReservationSystem.Api.Models;

namespace TicketReservationSystem.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly TicketContext _context;

        public BookingController(TicketContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> BookTicket(Booking booking)
        {
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.EventName == booking.EventName && e.Venue == booking.Venue);

            if (eventEntity == null || eventEntity.AvailableSeats < booking.SeatsBooked)
            {
                return BadRequest("Not enough available seats.");
            }

            eventEntity.AvailableSeats -= booking.SeatsBooked;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.Id }, booking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.EventName == booking.EventName && e.Venue == booking.Venue);
            if (eventEntity != null)
            {
                eventEntity.AvailableSeats += booking.SeatsBooked;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
