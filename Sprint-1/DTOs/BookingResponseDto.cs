using Sprint_1.Models;

namespace Sprint_1.DTOs;

public class BookingResponseDto
{
    public Guid Id { get; init; }
    public Guid EventId { get; init; }
    public BookingStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
}

