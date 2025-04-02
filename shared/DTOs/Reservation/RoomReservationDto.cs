namespace shared.DTOs.Reservation;

public class RoomReservationDto
{
    public int Id { get; set; }
    public DateTime ReservedFrom { get; set; }
    public DateTime ReservedTo { get; set; }
    public MemberDto ReservedBy { get; set; } = null!;
}
