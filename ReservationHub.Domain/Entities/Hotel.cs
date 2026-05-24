using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationHub.Domain.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StarRating { get; set; }

    // Navigation property — EF Core bu sayede JOIN yapabilir
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}