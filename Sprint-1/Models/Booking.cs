namespace Sprint_1.Models;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Booking() { }

    public static Booking CreatePending(Guid eventId)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Confirm()
    {
        Status = BookingStatus.Confirmed;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = BookingStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
    }

    public Booking Clone()
    {
        return new Booking
        {
            Id = Id,
            EventId = EventId,
            Status = Status,
            CreatedAt = CreatedAt,
            ProcessedAt = ProcessedAt
        };
    }
}

