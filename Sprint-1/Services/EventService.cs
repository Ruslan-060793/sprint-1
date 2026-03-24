using Sprint_1.DTOs;
using Sprint_1.Exceptions;
using Sprint_1.Models;

namespace Sprint_1.Services;

public class EventService : IEventService
{
    private readonly List<Event> _events = [];
    private readonly Lock _lock = new();

    public PaginatedResult<Event> GetAll(EventFilterDto filter)
    {
        lock (_lock)
        {
            var query = _events.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                query = query.Where(e =>
                    e.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.From.HasValue)
            {
                query = query.Where(e => e.StartAt >= filter.From.Value);
            }

            if (filter.To.HasValue)
            {
                query = query.Where(e => e.EndAt <= filter.To.Value);
            }

            var filtered = query.OrderBy(e => e.StartAt).ToList();
            var totalCount = filtered.Count;

            var page = Math.Max(1, filter.Page);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);

            var items = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(Clone)
                .ToList()
                .AsReadOnly();

            return new PaginatedResult<Event>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    public Event GetById(Guid id)
    {
        lock (_lock)
        {
            var existingEvent = _events.FirstOrDefault(e => e.Id == id);
            if (existingEvent is null)
            {
                throw new NotFoundException($"Event with id '{id}' was not found.");
            }

            return Clone(existingEvent);
        }
    }

    public Event Create(Event eventToCreate)
    {
        if (eventToCreate.EndAt <= eventToCreate.StartAt)
        {
            throw new Exceptions.ValidationException("EndAt must be later than StartAt.");
        }

        lock (_lock)
        {
            var createdEvent = Clone(eventToCreate);
            createdEvent.Id = Guid.NewGuid();

            _events.Add(createdEvent);

            return Clone(createdEvent);
        }
    }

    public Event Update(Guid id, Event updatedEvent)
    {
        if (updatedEvent.EndAt <= updatedEvent.StartAt)
        {
            throw new Exceptions.ValidationException("EndAt must be later than StartAt.");
        }

        lock (_lock)
        {
            var index = _events.FindIndex(e => e.Id == id);
            if (index < 0)
            {
                throw new NotFoundException($"Event with id '{id}' was not found.");
            }

            var eventToSave = Clone(updatedEvent);
            eventToSave.Id = id;
            _events[index] = eventToSave;

            return Clone(eventToSave);
        }
    }

    public void Delete(Guid id)
    {
        lock (_lock)
        {
            var existingEvent = _events.FirstOrDefault(e => e.Id == id);
            if (existingEvent is null)
            {
                throw new NotFoundException($"Event with id '{id}' was not found.");
            }

            _events.Remove(existingEvent);
        }
    }

    private static Event Clone(Event eventItem)
    {
        return new Event
        {
            Id = eventItem.Id,
            Title = eventItem.Title,
            Description = eventItem.Description,
            StartAt = eventItem.StartAt,
            EndAt = eventItem.EndAt
        };
    }
}