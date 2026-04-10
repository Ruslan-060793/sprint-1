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
            var booking = Booking.CreatePending(eventId);
            _bookings.Add(booking);

            return Task.FromResult(booking.Clone());
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

            return Task.FromResult(booking.Clone());
        }
    }

    public IReadOnlyCollection<Booking> GetPendingBookings()
    {
        lock (_lock)
        {
            return _bookings
                .Where(b => b.Status == BookingStatus.Pending)
                .Select(b => b.Clone())
                .ToList()
                .AsReadOnly();
        }
    }

    public void ConfirmBooking(Guid bookingId)
    {
        lock (_lock)
        {
            var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking is null)
            {
                throw new NotFoundException($"Бронирование с id '{bookingId}' не найдено.");
            }

            booking.Confirm();
        }
    }

    public void RejectBooking(Guid bookingId)
    {
        lock (_lock)
        {
            var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking is null)
            {
                throw new NotFoundException($"Бронирование с id '{bookingId}' не найдено.");
            }

            booking.Reject();
        }
    }

    public async Task ProcessBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        try
        {
            // Имитация обращения к внешней системе (оплата, проверка мест и т.д.)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            ConfirmBooking(booking.Id);
        }
        catch (OperationCanceledException)
        {
            throw; // Пробрасываем отмену дальше
        }
        catch (Exception)
        {
            RejectBooking(booking.Id);
            throw;
        }
    }
}

