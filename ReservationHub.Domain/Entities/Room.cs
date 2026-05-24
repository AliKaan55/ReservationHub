using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationHub.Domain.Entities;

public class Room : BaseEntity
{
    public int HotelId { get; set; }          // Foreign Key — hangi otele ait
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Hotel Hotel { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public enum RoomType
{
    Single = 1,
    Double = 2,
    Suite = 3,
    Deluxe = 4
}
