using ReservationHub.Application.Caching;
using ReservationHub.Application.DTOs;
using ReservationHub.Application.Interfaces;
using ReservationHub.Domain.Entities;
using ReservationHub.Domain.Interfaces;

namespace ReservationHub.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly CacheService _cacheService;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        CacheService cacheService)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _cacheService = cacheService;
    }

    public async Task<ReservationDto> CreateAsync(int userId, CreateReservationDto dto)
    {
        if (dto.CheckInDate >= dto.CheckOutDate)
            throw new Exception("Çıkış tarihi, giriş tarihinden sonra olmalıdır.");

        if (dto.CheckInDate < DateTime.UtcNow.Date)
            throw new Exception("Geçmiş bir tarihe rezervasyon yapılamaz.");

        var room = await _roomRepository.GetByIdAsync(dto.RoomId)
            ?? throw new Exception("Oda bulunamadı.");

        if (!room.IsActive)
            throw new Exception("Bu oda şu an müsait değil.");

        var conflictingReservations = await _reservationRepository
            .GetByRoomAndDateRangeAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);

        if (conflictingReservations.Any())
            throw new Exception("Seçilen tarihler için oda müsait değil.");

        var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
        var totalPrice = room.PricePerNight * nights;

        var reservation = new Reservation
        {
            RoomId = dto.RoomId,
            UserId = userId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            TotalPrice = totalPrice,
            Status = ReservationStatus.Confirmed,
            Notes = dto.Notes
        };

        await _reservationRepository.AddAsync(reservation);
        await _reservationRepository.SaveChangesAsync();

        await _cacheService.RemoveByPrefixAsync($"available_rooms:1:");

        return MapToDto(reservation, room);
    }

    public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId)
    {
        var reservations = await _reservationRepository.GetByUserIdAsync(userId);

        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            RoomId = r.RoomId,
            RoomNumber = r.Room?.RoomNumber ?? string.Empty,
            HotelName = r.Room?.Hotel?.Name ?? string.Empty,
            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            TotalPrice = r.TotalPrice,
            Status = r.Status.ToString(),
            Notes = r.Notes
        });
    }

    public async Task<ReservationDto?> GetByIdAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetWithDetailsAsync(reservationId);
        if (reservation is null) return null;

        return new ReservationDto
        {
            Id = reservation.Id,
            RoomId = reservation.RoomId,
            RoomNumber = reservation.Room?.RoomNumber ?? string.Empty,
            HotelName = reservation.Room?.Hotel?.Name ?? string.Empty,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            TotalPrice = reservation.TotalPrice,
            Status = reservation.Status.ToString(),
            Notes = reservation.Notes
        };
    }

    public async Task<bool> CancelAsync(int reservationId, int userId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation is null) return false;

        if (reservation.UserId != userId)
            throw new Exception("Bu rezervasyonu iptal etme yetkiniz yok.");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new Exception("Bu rezervasyon zaten iptal edilmiş.");

        if (reservation.CheckInDate <= DateTime.UtcNow.AddHours(24))
            throw new Exception("Check-in'e 24 saatten az kaldığında iptal edilemez.");

        reservation.Status = ReservationStatus.Cancelled;
        reservation.UpdatedAt = DateTime.UtcNow;

        _reservationRepository.Update(reservation);
        await _reservationRepository.SaveChangesAsync();

        return true;
    }

    private static ReservationDto MapToDto(Reservation reservation, Room room) => new()
    {
        Id = reservation.Id,
        RoomId = reservation.RoomId,
        RoomNumber = room.RoomNumber,
        HotelName = string.Empty,
        CheckInDate = reservation.CheckInDate,
        CheckOutDate = reservation.CheckOutDate,
        TotalPrice = reservation.TotalPrice,
        Status = reservation.Status.ToString(),
        Notes = reservation.Notes
    };
}