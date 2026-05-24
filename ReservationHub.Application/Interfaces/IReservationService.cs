using ReservationHub.Application.DTOs;

namespace ReservationHub.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationDto> CreateAsync(int userId, CreateReservationDto dto);
    Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId);
    Task<ReservationDto?> GetByIdAsync(int reservationId);
    Task<bool> CancelAsync(int reservationId, int userId);
}