using Microsoft.AspNetCore.Mvc;
using Sprint_1.DTOs;
using Sprint_1.Models;
using Sprint_1.Services;

namespace Sprint_1.Controllers;

[ApiController]
[Route("events")]
public class EventsController(IEventService eventService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<EventResponseDto>> GetAll()
    {
        var events = eventService.GetAll().Select(MapToResponse);
        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EventResponseDto> GetById(Guid id)
    {
        var eventItem = eventService.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(eventItem));
    }

    [HttpPost]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<EventResponseDto> Create([FromBody] EventRequestDto request)
    {
        var eventToCreate = MapToModel(request);
        var createdEvent = eventService.Create(eventToCreate);

        return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, MapToResponse(createdEvent));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EventResponseDto> Update(Guid id, [FromBody] EventRequestDto request)
    {
        var eventToUpdate = MapToModel(request);
        var updatedEvent = eventService.Update(id, eventToUpdate);

        if (updatedEvent is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(updatedEvent));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var deleted = eventService.Delete(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static Event MapToModel(EventRequestDto request)
    {
        return new Event
        {
            Title = request.Title!.Trim(),
            Description = request.Description,
            StartAt = request.StartAt!.Value,
            EndAt = request.EndAt!.Value
        };
    }

    private static EventResponseDto MapToResponse(Event eventItem)
    {
        return new EventResponseDto
        {
            Id = eventItem.Id,
            Title = eventItem.Title,
            Description = eventItem.Description,
            StartAt = eventItem.StartAt,
            EndAt = eventItem.EndAt
        };
    }
}