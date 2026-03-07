using Sprint_1.Models;

namespace Sprint_1.Services;

public interface IEventService
{
    IReadOnlyCollection<Event> GetAll();

    Event? GetById(Guid id);

    Event Create(Event eventToCreate);

    Event? Update(Guid id, Event updatedEvent);

    bool Delete(Guid id);
}