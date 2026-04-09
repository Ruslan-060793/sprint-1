using Sprint_1.Models;

namespace Sprint_1.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Guid eventId);
    Task<Booking> GetBookingByIdAsync(Guid bookingId);
    IReadOnlyCollection<Booking> GetPendingBookings();
    void UpdateBooking(Booking booking);
}

