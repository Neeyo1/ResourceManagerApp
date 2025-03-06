namespace API.DTOs.Reservation;

public class RoomReservationUpdateDto
{
    public DateTime? ReservedFrom { get; set; }
    public DateTime? ReservedTo { get; set; }
}
