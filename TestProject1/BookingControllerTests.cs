using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketReservationSystem.Api.Controllers;
using TicketReservationSystem.Api.Models;
using TicketReservationSystem.Api.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject1
{
    [TestFixture]
    public class BookingControllerTests
    {
        private TicketContext _context;
        private BookingController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TicketContext>()
                .UseInMemoryDatabase(databaseName: "TicketReservationTestDb_Booking")
                .Options;

            _context = new TicketContext(options);
            SeedDatabase();
            _controller = new BookingController(_context);
        }

        private void SeedDatabase()
        {
            _context.Events.AddRange(new List<Event>
            {
                new Event { EventName = "Concert", Venue = "Auditorium A", AvailableSeats = 195 },
                new Event { EventName = "Music Festival", Venue = "Open Arena", AvailableSeats = 200 }
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task BookTicket_WhenSeatsAvailable_ReturnsCreatedAtAction()
        {
            var booking = new Booking { EventName = "Concert", Venue = "Auditorium A", SeatsBooked = 2 };

            var result = await _controller.BookTicket(booking);

            Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task BookTicket_WhenNotEnoughSeats_ReturnsBadRequest()
        {
            var booking = new Booking { EventName = "Music Festival", Venue = "Open Arena", SeatsBooked = 250 };

            var result = await _controller.BookTicket(booking);

            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CancelBooking_WhenBookingExists_ReturnsNoContent()
        {
            var booking = new Booking { Id = 3, EventName = "Concert", Venue = "Auditorium A", SeatsBooked = 3 };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var result = await _controller.CancelBooking(3);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task CancelBooking_WhenBookingDoesNotExist_ReturnsNotFound()
        {
            var result = await _controller.CancelBooking(100);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }

    [TestFixture]
    public class EventControllerTests
    {
        private TicketContext _context;
        private EventController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TicketContext>()
                .UseInMemoryDatabase(databaseName: "TicketReservationTestDb_Event")
                .Options;

            _context = new TicketContext(options);
            SeedDatabase();
            _controller = new EventController(_context);
        }

        private void SeedDatabase()
        {
            _context.Events.AddRange(new List<Event>
            {
                new Event { Id = 1, EventName = "Concert", Venue = "Auditorium A", AvailableSeats = 195 },
                new Event { Id = 2, EventName = "Theater Play", Venue = "City Hall", AvailableSeats = 131 },
                new Event { Id = 3, EventName = "Music Festival", Venue = "Open Arena", AvailableSeats = 200 }
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetEvents_ReturnsAllBookings()
        {
            // Act
            var result = await _controller.GetEvents();

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<Booking>>>()); // Expecting Booking instead of Event
            Assert.That(result.Value, Has.Exactly(1).Matches<Booking>(b => b.EventName == "Concert"));
            Assert.That(result.Value, Has.Exactly(1).Matches<Booking>(b => b.EventName == "Theater Play"));
            Assert.That(result.Value, Has.Exactly(1).Matches<Booking>(b => b.EventName == "Music Festival"));
        }



        [Test]
        public async Task GetUniqueVenues_ReturnsDistinctVenues()
        {
            var result = await _controller.GetUniqueVenues();

            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<string>>>());
            Assert.That(result.Value.Count(), Is.EqualTo(3));
            Assert.That(result.Value, Does.Contain("Auditorium A"));
            Assert.That(result.Value, Does.Contain("City Hall"));
            Assert.That(result.Value, Does.Contain("Open Arena"));
        }
    }
}
