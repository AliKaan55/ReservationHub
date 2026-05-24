using Microsoft.AspNetCore.Mvc;
using ReservationHub.Application.Interfaces;

namespace ReservationHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] int hotelId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut)
    {
        try
        {
            var rooms = await _roomService.GetAvailableRoomsAsync(hotelId, checkIn, checkOut);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<IActionResult> GetByHotel(int hotelId)
    {
        var rooms = await _roomService.GetByHotelIdAsync(hotelId);
        return Ok(rooms);
    }
}