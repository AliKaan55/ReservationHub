using ReservationHub.Domain.Entities;

namespace ReservationHub.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{
    // Bir odanın belirli tarih aralığındaki rezervasyonlarını getir
    Task<IEnumerable<Reservation>> GetByRoomAndDateRangeAsync(
        int roomId,
        DateTime checkIn,
        DateTime checkOut);

    // Kullanıcının tüm rezervasyonlarını getir
    Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);

    // Detaylı rezervasyon — Room ve User bilgileriyle birlikte
    Task<Reservation?> GetWithDetailsAsync(int reservationId);
}