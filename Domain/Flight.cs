
namespace Domain
{
    public class Flight
    {

        List<Booking> bookingList = new();
        public IEnumerable<Booking> BookingList => bookingList;


        //public List<Booking> BookingList { get; set; } = new List<Booking>();

        public int RemainingNumberOfSeats { get; set; }

        public Guid Id { get; private set; }

        [Obsolete("Needed by EF")]
        Flight() {}

        public Flight(int seatCapacity)
        {
            Id = Guid.NewGuid();
            RemainingNumberOfSeats = seatCapacity;
        }

        public object? Book(string passengerEmail, int numberOfSeats)  //nullable so it CAN return an object but it doesnt have to.
        {
            if (numberOfSeats > this.RemainingNumberOfSeats)
                return new OverbookingError();

            RemainingNumberOfSeats -= numberOfSeats;

            bookingList.Add(new Booking(passengerEmail, numberOfSeats));

            return null;
        }

        public object? CancelBooking(string passengerEmail, int numberOfSeats)
        {
            var booking = bookingList.FirstOrDefault(booking => booking.Email == passengerEmail);

            if (booking is null)
                return new BookingNotFoundError();

            //You cannot hand back more seats than you actually hold.
            if (numberOfSeats > booking.NumberOfSeats)
                return new CannotCancelMoreSeatsThanBookedError();

            booking.NumberOfSeats -= numberOfSeats;

            //A fully cancelled booking no longer exists.
            if (booking.NumberOfSeats == 0)
                bookingList.Remove(booking);

            RemainingNumberOfSeats += numberOfSeats;
            return null;
        }
    }
}
