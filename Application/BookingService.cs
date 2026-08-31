using Data;
using Domain;

namespace Application
{
    public class BookingService
    {
        public Entities Entities { get; set; }

        public BookingService(Entities entities)
        {
            Entities = entities;
        }

        public object? Book(BookDto bookDto)
        {
            var flight = Entities.Flights.Find(bookDto.FlightId);

            if (flight is null)
                return new FlightNotFoundError();

            var error = flight.Book(bookDto.PassengerEmail, bookDto.NumberOfSeats);

            //Nothing was booked, so there is nothing to persist.
            if (error != null)
                return error;

            Entities.SaveChanges();
            return null;
        }

        public IEnumerable<BookingRm> FindBookings(Guid flightId)
        {
            var flight = Entities.Flights.Find(flightId);

            if (flight is null)
                return Enumerable.Empty<BookingRm>();

            return flight.BookingList.Select(booking => new BookingRm(booking.Email, booking.NumberOfSeats));
        }

        public object? CancelBooking(CancelBookingDto cancelbookingDto)
        {
            var flight = Entities.Flights.Find(cancelbookingDto.FlightId);

            if (flight is null)
                return new FlightNotFoundError();

            var error = flight.CancelBooking(cancelbookingDto.PassengerEmail, cancelbookingDto.NumberOfSeats);

            if (error != null)
                return error;

            Entities.SaveChanges();
            return null;
        }

        public int? GetRemainingNumberOfSeatsFor(Guid flightId)
        {
            return Entities.Flights.Find(flightId)?.RemainingNumberOfSeats;
        }
    }
}
