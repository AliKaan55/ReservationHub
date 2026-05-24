using Microsoft.EntityFrameworkCore;
using ReservationHub.Domain.Entities;
using ReservationHub.Domain.Interfaces;
using ReservationHub.Infrastructure.Persistence;

namespace ReservationHub.Infrastructure.Repositories;

public class RoomRepository : Repository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(
        int hotelId, DateTime checkIn, DateTime checkOut)
    {
        // Çakışan rezervasyonu olan oda id'lerini bul
        var reservedRoomIds = await _context.Reservations
            .Where(r => r.Status != ReservationStatus.Cancelled
                && r.CheckInDate < checkOut
                && r.CheckOutDate > checkIn)
            .Select(r => r.RoomId)
            .Distinct()
            .ToListAsync();

        // O id'lerde olmayan, aktif odaları getir
        return await _context.Rooms
            .Where(r => r.HotelId == hotelId
                && r.IsActive
                && !reservedRoomIds.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId)
    {
        return await _context.Rooms
            .Where(r => r.HotelId == hotelId && r.IsActive)
            .ToListAsync();
    }
}