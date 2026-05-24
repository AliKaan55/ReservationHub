using ReservationHub.Domain.Entities;

namespace ReservationHub.Domain.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    // Belirli tarihte müsait odaları getir
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(
        int hotelId,
        DateTime checkIn,
        DateTime checkOut);

    // Otel id'sine göre tüm odaları getir
    Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId);
}