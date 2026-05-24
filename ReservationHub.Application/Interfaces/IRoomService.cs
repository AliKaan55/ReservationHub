using ReservationHub.Application.DTOs;

namespace ReservationHub.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(
        int hotelId, DateTime checkIn, DateTime checkOut);
    Task<IEnumerable<RoomDto>> GetByHotelIdAsync(int hotelId);
}