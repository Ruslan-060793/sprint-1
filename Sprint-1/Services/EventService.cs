using Sprint_1.Models;

namespace Sprint_1.Services;

public class EventService : IEventService
{
    private readonly List<Event> _events = [];
    private readonly Lock _lock = new();

    public IReadOnlyCollection<Event> GetAll()
    {
        lock (_lock)
        {
            return _events
                .OrderBy(e => e.StartAt)
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public Event? GetById(Guid id)
    {
        lock (_lock)
        {
            var existingEvent = _events.FirstOrDefault(e => e.Id == id);
            return existingEvent is null ? null : Clone(existingEvent);
        }
    }

    public Event Create(Event eventToCreate)
    {
        lock (_lock)
        {
            var createdEvent = Clone(eventToCreate);
            createdEvent.Id = Guid.NewGuid();

            _events.Add(createdEvent);

            return Clone(createdEvent);
        }
    }

    public Event? Update(Guid id, Event updatedEvent)
    {
        lock (_lock)
        {
            var index = _events.FindIndex(e => e.Id == id);
            if (index < 0)
            {
                return null;
            }

            var eventToSave = Clone(updatedEvent);
            eventToSave.Id = id;
            _events[index] = eventToSave;

            return Clone(eventToSave);
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            var existingEvent = _events.FirstOrDefault(e => e.Id == id);
            if (existingEvent is null)
            {
                return false;
            }

            _events.Remove(existingEvent);
            return true;
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