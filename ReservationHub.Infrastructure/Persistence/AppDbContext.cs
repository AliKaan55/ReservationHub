using Microsoft.EntityFrameworkCore;
using ReservationHub.Domain.Entities;

namespace ReservationHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Her DbSet = veritabanında bir tablo
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Hotel konfigürasyonu
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
            entity.Property(h => h.City).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Address).IsRequired().HasMaxLength(500);
        });

        // Room konfigürasyonu
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.PricePerNight).HasColumnType("decimal(18,2)");
            entity.Property(r => r.RoomNumber).IsRequired().HasMaxLength(10);

            // Hotel ile ilişki: 1 Hotel → N Room
            entity.HasOne(r => r.Hotel)
                  .WithMany(h => h.Rooms)
                  .HasForeignKey(r => r.HotelId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // User konfigürasyonu
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique(); // Email tekrar edemez
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        // Reservation konfigürasyonu
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.TotalPrice).HasColumnType("decimal(18,2)");

            // Room ile ilişki
            entity.HasOne(r => r.Room)
                  .WithMany(room => room.Reservations)
                  .HasForeignKey(r => r.RoomId)
                  .OnDelete(DeleteBehavior.Restrict); // Oda silinince rezervasyon silinmesin

            // User ile ilişki
            entity.HasOne(r => r.User)
                  .WithMany(u => u.Reservations)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}   