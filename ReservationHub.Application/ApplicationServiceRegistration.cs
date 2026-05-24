using Microsoft.Extensions.DependencyInjection;
using ReservationHub.Application.Caching;
using ReservationHub.Application.Interfaces;
using ReservationHub.Application.Services;

namespace ReservationHub.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<CacheService>();  // ← ekle

        return services;
    }
}