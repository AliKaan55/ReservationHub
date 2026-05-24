using Microsoft.EntityFrameworkCore;
using ReservationHub.Domain.Entities;
using ReservationHub.Domain.Interfaces;
using ReservationHub.Infrastructure.Persistence;

namespace ReservationHub.Infrastructure.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Reservation>> GetByRoomAndDateRangeAsync(
        int roomId, DateTime checkIn, DateTime checkOut)
    {
        return await _context.Reservations
            .Where(r => r.RoomId == roomId
                && r.Status != ReservationStatus.Cancelled
                && r.CheckInDate < checkOut      // mevcut rezervasyon bizim checkOut'tan önce başlıyor
                && r.CheckOutDate > checkIn)     // mevcut rezervasyon bizim checkIn'den sonra bitiyor
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
    {
        return await _context.Reservations
            .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Reservation?> GetWithDetailsAsync(int reservationId)
    {
        return await _context.Reservations
            .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reservationId);
    }
}