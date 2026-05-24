using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationHub.Domain.Entities;

public class Reservation : BaseEntity
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }

    // Navigation properties
    public Room Room { get; set; } = null!;
    public User User { get; set; } = null!;
}

public enum ReservationStatus
{
    Pending = 1,  // Onay bekliyor
    Confirmed = 2,  // Onaylandı
    Cancelled = 3,  // İptal edildi
    Completed = 4   // Tamamlandı
}
