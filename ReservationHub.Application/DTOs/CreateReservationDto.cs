namespace ReservationHub.Application.DTOs;

public class CreateReservationDto
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string? Notes { get; set; }
}