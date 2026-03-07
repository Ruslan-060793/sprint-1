namespace Sprint_1.DTOs;

public class EventResponseDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTime StartAt { get; init; }

    public DateTime EndAt { get; init; }
}