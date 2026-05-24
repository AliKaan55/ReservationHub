using ReservationHub.Application.DTOs;
using ReservationHub.Application.Interfaces;
using ReservationHub.Domain.Interfaces;
using ReservationHub.Application.Caching;    

namespace ReservationHub.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly CacheService _cacheService;

    public RoomService(IRoomRepository roomRepository, CacheService cacheService)
    {
        _roomRepository = roomRepository;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(
        int hotelId, DateTime checkIn, DateTime checkOut)
    {
        // Cache key — her farklı sorgu için benzersiz anahtar
        var cacheKey = $"available_rooms:{hotelId}:{checkIn:yyyyMMdd}:{checkOut:yyyyMMdd}";

        // Önce cache'e bak
        var cached = await _cacheService.GetAsync<IEnumerable<RoomDto>>(cacheKey);
        if (cached is not null)
        {
            return cached; // DB'ye gitme, cache'den dön
        }

        // Cache'de yoksa DB'den çek
        var rooms = await _roomRepository
            .GetAvailableRoomsAsync(hotelId, checkIn, checkOut);

        var result = rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            Type = r.Type.ToString(),
            PricePerNight = r.PricePerNight,
            Capacity = r.Capacity
        }).ToList();

        // Cache'e yaz — 5 dakika geçerli
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }

    public async Task<IEnumerable<RoomDto>> GetByHotelIdAsync(int hotelId)
    {
        var cacheKey = $"hotel_rooms:{hotelId}";

        var cached = await _cacheService.GetAsync<IEnumerable<RoomDto>>(cacheKey);
        if (cached is not null) return cached;

        var rooms = await _roomRepository.GetByHotelIdAsync(hotelId);

        var result = rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            Type = r.Type.ToString(),
            PricePerNight = r.PricePerNight,
            Capacity = r.Capacity
        }).ToList();

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

        return result;
    }
}