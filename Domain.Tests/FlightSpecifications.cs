using FluentAssertions;
using Domain;

namespace Domain.Tests
{
    public class FlightSpecifications
    {
        [Theory]
        [InlineData(3, 1, 2)]
        [InlineData(10, 6, 4)]
        [InlineData(6, 3, 3)]
        [InlineData(12, 8, 4)]
        public void Booking_reduces_the_number_of_seats(int seatCapacity, int numberOfSeats, int remainingNumberOfSeats)
        {
            //Given - Preconditions
            var flight = new Flight(seatCapacity: seatCapacity);
            //When - Triggers the scenario
            flight.Book("email@email.com", numberOfSeats);
            //Then - Expected outcome
            flight.RemainingNumberOfSeats.Should().Be(remainingNumberOfSeats);
        }

        [Fact]
        public void Avoid_Overbooking()
        {
            //Given
            var flight = new Flight(seatCapacity: 3);

            //When
            var error = flight.Book("passenger@email.com", 4);

            //Then
            error.Should().BeOfType<OverbookingError>();
        }

        [Fact]
        public void Books_Flights_Successfully()
        {
            var flight = new Flight(seatCapacity: 3);
            var error = flight.Book("email@email.com", 1);
            error.Should().BeNull();
        }

        [Fact]
        public void Remember_Bookings()
        {
            var flight = new Flight(seatCapacity: 150);

            flight.Book(passengerEmail: "callan@email.com", numberOfSeats: 2);

            flight.BookingList.Should().ContainEquivalentOf(new Booking("callan@email.com", 2));
        }

        [Theory]
        [InlineData(3, 1, 1, 3)]
        [InlineData(150, 15, 10, 145)]
        public void Cancelling_Bookings_Frees_Up_Seats(int initialCapacity, int numberOfSeatsToBook, int numbeOfSeatsToCancel, int remainingNumberOfSeats)
        {
            //given
            var flight = new Flight(initialCapacity);
            flight.Book(passengerEmail: "david@email.com", numberOfSeatsToBook);

            //when
            flight.CancelBooking(passengerEmail: "david@email.com", numbeOfSeatsToCancel);

            //then
            flight.RemainingNumberOfSeats.Should().Be(remainingNumberOfSeats);
        }

        [Fact]
        public void Doesnt_cancel_bookings_for_passengers_who_have_not_booked()
        {
            var flight = new Flight(3);
            var error = flight.CancelBooking(passengerEmail: "email@email.com", numberOfSeats: 2);
            error.Should().BeOfType<BookingNotFoundError>();
        }
        [Fact]
        public void Returns_null_when_successfully_cancelled_booking()
        {
            var flight = new Flight(3);
            flight.Book(passengerEmail: "email@email.com", numberOfSeats: 1);
            var error = flight.CancelBooking(passengerEmail: "email@email.com", numberOfSeats: 1);
            error.Should().Be(null);
        }

        [Fact]
        public void Cancelling_the_same_booking_twice_does_not_invent_seats()
        {
            //given
            var flight = new Flight(seatCapacity: 3);
            flight.Book(passengerEmail: "email@email.com", numberOfSeats: 2);

            //when - the same booking is cancelled a second time
            flight.CancelBooking(passengerEmail: "email@email.com", numberOfSeats: 2);
            flight.CancelBooking(passengerEmail: "email@email.com", numberOfSeats: 2);

            //then - the flight never exceeds the capacity it was built with
            flight.RemainingNumberOfSeats.Should().Be(3);
        }

        [Fact]
        public void Doesnt_cancel_more_seats_than_were_booked()
        {
            var flight = new Flight(seatCapacity: 10);
            flight.Book(passengerEmail: "email@email.com", numberOfSeats: 2);

            var error = flight.CancelBooking(passengerEmail: "email@email.com", numberOfSeats: 5);

            error.Should().BeOfType<CannotCancelMoreSeatsThanBookedError>();
            flight.RemainingNumberOfSeats.Should().Be(8);
        }

        [Fact]
        public void Forgets_a_booking_once_it_is_fully_cancelled()
        {
            var flight = new Flight(seatCapacity: 3);
            flight.Book(passengerEmail: "email@email.com", numberOfSeats: 2);

            flight.CancelBooking(passengerEmail: "email@email.com", numberOfSeats: 2);

            flight.BookingList.Should().BeEmpty();
        }
    }
}
