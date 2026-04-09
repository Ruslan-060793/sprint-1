using Sprint_1.Exceptions;
using Sprint_1.Models;
using Sprint_1.Services;

namespace Sprint_1.Tests;

public class BookingServiceTests
{
    private readonly EventService _eventService = new();
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingService = new BookingService(_eventService);
    }

    private Event CreateAndStoreEvent(string title = "Тестовое событие")
    {
        return _eventService.Create(new Event
        {
            Title = title,
            Description = "Описание",
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2)
        });
    }

    // ===== Успешные сценарии =====

    [Fact]
    public async Task CreateBookingAsync_ExistingEvent_ReturnsBookingWithPendingStatus()
    {
        // Arrange
        var ev = CreateAndStoreEvent();

        // Act
        var booking = await _bookingService.CreateBookingAsync(ev.Id);

        // Assert
        Assert.NotEqual(Guid.Empty, booking.Id);
        Assert.Equal(ev.Id, booking.EventId);
        Assert.Equal(BookingStatus.Pending, booking.Status);
        Assert.True(booking.CreatedAt <= DateTime.UtcNow);
        Assert.Null(booking.ProcessedAt);
    }

    [Fact]
    public async Task CreateBookingAsync_MultipleBookings_HaveUniqueIds()
    {
        // Arrange
        var ev = CreateAndStoreEvent();

        // Act
        var booking1 = await _bookingService.CreateBookingAsync(ev.Id);
        var booking2 = await _bookingService.CreateBookingAsync(ev.Id);
        var booking3 = await _bookingService.CreateBookingAsync(ev.Id);

        // Assert
        var ids = new[] { booking1.Id, booking2.Id, booking3.Id };
        Assert.Equal(3, ids.Distinct().Count());
    }

    [Fact]
    public async Task GetBookingByIdAsync_ExistingBooking_ReturnsCorrectInfo()
    {
        // Arrange
        var ev = CreateAndStoreEvent();
        var created = await _bookingService.CreateBookingAsync(ev.Id);

        // Act
        var fetched = await _bookingService.GetBookingByIdAsync(created.Id);

        // Assert
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal(created.EventId, fetched.EventId);
        Assert.Equal(BookingStatus.Pending, fetched.Status);
    }

    [Fact]
    public async Task UpdateBooking_ChangesStatus_ReflectedInGetById()
    {
        // Arrange
        var ev = CreateAndStoreEvent();
        var created = await _bookingService.CreateBookingAsync(ev.Id);

        // Act
        var toUpdate = await _bookingService.GetBookingByIdAsync(created.Id);
        toUpdate.Status = BookingStatus.Confirmed;
        toUpdate.ProcessedAt = DateTime.UtcNow;
        _bookingService.UpdateBooking(toUpdate);

        // Assert
        var updated = await _bookingService.GetBookingByIdAsync(created.Id);
        Assert.Equal(BookingStatus.Confirmed, updated.Status);
        Assert.NotNull(updated.ProcessedAt);
    }

    [Fact]
    public async Task GetPendingBookings_ReturnsOnlyPending()
    {
        // Arrange
        var ev = CreateAndStoreEvent();
        var booking1 = await _bookingService.CreateBookingAsync(ev.Id);
        await _bookingService.CreateBookingAsync(ev.Id);

        // Подтверждаем первую
        var toConfirm = await _bookingService.GetBookingByIdAsync(booking1.Id);
        toConfirm.Status = BookingStatus.Confirmed;
        toConfirm.ProcessedAt = DateTime.UtcNow;
        _bookingService.UpdateBooking(toConfirm);

        // Act
        var pending = _bookingService.GetPendingBookings();

        // Assert
        Assert.Single(pending);
        Assert.All(pending, b => Assert.Equal(BookingStatus.Pending, b.Status));
    }

    // ===== Неуспешные сценарии =====

    [Fact]
    public async Task CreateBookingAsync_NonExistentEvent_ThrowsNotFoundException()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => _bookingService.CreateBookingAsync(Guid.NewGuid()));
        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateBookingAsync_DeletedEvent_ThrowsNotFoundException()
    {
        // Arrange
        var ev = CreateAndStoreEvent();
        _eventService.Delete(ev.Id);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => _bookingService.CreateBookingAsync(ev.Id));
        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetBookingByIdAsync_NonExistentId_ThrowsNotFoundException()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => _bookingService.GetBookingByIdAsync(Guid.NewGuid()));
        Assert.Contains("не найден", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}

