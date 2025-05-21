using System.Collections.Generic;
using FluentAssertions;
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;


namespace Application.Tests
{
    public class FlightApplicationSpecifications
    {
        /*
         So when you call Book(), data should be stored in a database 
         and when you call FindBookings(), it should return the data.
         */

        readonly Entities entities = new Entities(new DbContextOptionsBuilder<Entities>().UseInMemoryDatabase("Flights").Options);
        readonly BookingService bookingService;
        public FlightApplicationSpecifications()
        {
            bookingService = new BookingService(entities: entities);
        }

        [Theory]
        [InlineData("email@email.com", 2)]
        [InlineData("gmail@email.com", 2)]

        public void Remembers_bookings(string passengerEmail, int numberOfSeats)
        {

            var flight = new Flight(3);

            entities.Flights.Add(flight);

            bookingService.Book(
                new BookDto(flightId: flight.Id, passengerEmail: passengerEmail, numberOfSeats: numberOfSeats)
                );  // create a DTO - data transfer object

            bookingService.FindBookings(flight.Id).Should().ContainEquivalentOf(
                new BookingRm(passengerEmail: passengerEmail, numberOfSeats: numberOfSeats)
                );  // BookingRm is a Read Model that also contains information.
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]

        public void Frees_up_seats_after_booking(int initialCapacity)
        {
            //given
            var flight = new Flight(initialCapacity);
            entities.Flights.Add(flight);

            bookingService.Book(
                new BookDto(flightId: flight.Id, passengerEmail: "email@email.com", numberOfSeats: 2)
                );

            //when
            bookingService.CancelBooking(
                new CancelBookingDto(flightId: Guid.NewGuid(), passengerEmail: "email@email.com", numberOfSeats: 2)
                );

            //then
            bookingService.GetRemainingNumberOfSeatsFor(flight.Id).Should().Be(initialCapacity);
        }
    }
}
