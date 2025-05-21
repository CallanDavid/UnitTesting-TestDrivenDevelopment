using Data;

namespace Application
{
    public class BookingService
    {
        public Entities Entities { get; set; }

        public BookingService(Entities entities)
        {
            Entities = entities;
        }

        public void Book(BookDto bookDto)
        {
            var flight = Entities.Flights.Find(bookDto.FlightId);
            flight.Book(bookDto.PassengerEmail, bookDto.NumberOfSeats);
        }

        public IEnumerable<BookingRm> FindBookings(Guid flightId)
        {
            return Entities.Flights.Find(flightId).BookingList.Select(booking => new BookingRm(booking.Email, booking.NumberOfSeats));
        }

        public void CancelBooking(CancelBookingDto cancelbookingDto)
        {
            var flight = Entities.Flights.Find(cancelbookingDto.FlightId);
            flight.CancelBooking(cancelbookingDto.PassengerEmail, cancelbookingDto.NumberOfSeats);
            Entities.SaveChanges();
        }

        public object? GetRemainingNumberOfSeatsFor(Guid flightId)
        {
            return Entities.Flights.Find(flightId).RemainingNumberOfSeats;
        }
    }
}
