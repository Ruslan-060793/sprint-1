using Microsoft.AspNetCore.Mvc;
using Sprint_1.DTOs;
using Sprint_1.Models;
using Sprint_1.Services;

namespace Sprint_1.Controllers;

[ApiController]
[Route("bookings")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponseDto>> GetById(Guid id)
    {
        var booking = await bookingService.GetBookingByIdAsync(id);
        return Ok(MapToResponse(booking));
    }

    private static BookingResponseDto MapToResponse(Booking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            EventId = booking.EventId,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt,
            ProcessedAt = booking.ProcessedAt
        };
    }
}

