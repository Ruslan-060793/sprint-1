namespace Sprint_1.DTOs;

public class PaginatedResult<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

