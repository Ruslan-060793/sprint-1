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
    [ProducesResponseType(typeof(PaginatedResult<EventResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<PaginatedResult<EventResponseDto>> GetAll([FromQuery] EventFilterDto filter)
    {
        var result = eventService.GetAll(filter);

        var response = new PaginatedResult<EventResponseDto>
        {
            Items = result.Items.Select(MapToResponse).ToList().AsReadOnly(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EventResponseDto> GetById(Guid id)
    {
        var eventItem = eventService.GetById(id);
        return Ok(MapToResponse(eventItem));
    }

    [HttpPost]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<EventResponseDto> Create([FromBody] EventRequestDto request)
    {
        var eventToCreate = MapToModel(request);
        var createdEvent = eventService.Create(eventToCreate);

        return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, MapToResponse(createdEvent));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EventResponseDto> Update(Guid id, [FromBody] EventRequestDto request)
    {
        var eventToUpdate = MapToModel(request);
        var updatedEvent = eventService.Update(id, eventToUpdate);

        return Ok(MapToResponse(updatedEvent));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        eventService.Delete(id);
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