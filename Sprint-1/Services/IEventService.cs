using Sprint_1.DTOs;
using Sprint_1.Models;

namespace Sprint_1.Services;

public interface IEventService
{
    PaginatedResult<Event> GetAll(EventFilterDto filter);

    Event GetById(Guid id);

    Event Create(Event eventToCreate);

    Event Update(Guid id, Event updatedEvent);

    void Delete(Guid id);
}