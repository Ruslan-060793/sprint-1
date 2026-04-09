using Sprint_1.Exceptions;
using Sprint_1.Models;

namespace Sprint_1.Services;

public class BookingService(IEventService eventService) : IBookingService
{
    private readonly List<Booking> _bookings = [];
    private readonly Lock _lock = new();

    public Task<Booking> CreateBookingAsync(Guid eventId)
    {
        // Проверяем, что событие существует (бросит NotFoundException если нет)
        eventService.GetById(eventId);

        lock (_lock)
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _bookings.Add(booking);

            return Task.FromResult(Clone(booking));
        }
    }

    public Task<Booking> GetBookingByIdAsync(Guid bookingId)
    {
        lock (_lock)
        {
            var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking is null)
            {
                throw new NotFoundException($"Бронирование с id '{bookingId}' не найдено.");
            }

            return Task.FromResult(Clone(booking));
        }
    }

    public IReadOnlyCollection<Booking> GetPendingBookings()
    {
        lock (_lock)
        {
            return _bookings
                .Where(b => b.Status == BookingStatus.Pending)
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public void UpdateBooking(Booking booking)
    {
        lock (_lock)
        {
            var index = _bookings.FindIndex(b => b.Id == booking.Id);
            if (index < 0)
            {
                throw new NotFoundException($"Бронирование с id '{booking.Id}' не найдено.");
            }

            _bookings[index] = Clone(booking);
        }
    }

    private static Booking Clone(Booking booking)
    {
        return new Booking
        {
            Id = booking.Id,
            EventId = booking.EventId,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt,
            ProcessedAt = booking.ProcessedAt
        };
    }
}

