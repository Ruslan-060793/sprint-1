namespace Sprint_1.DTOs;

public class EventFilterDto
{
    public string? Title { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

