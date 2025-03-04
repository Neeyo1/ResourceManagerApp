namespace API.Entities;

public class RoomReservation
{
    public int Id { get; set; }
    public DateTime ReservedFrom { get; set; }
    public DateTime ReservedTo { get; set; }

    //RoomReservation - Room
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    //RoomReservation - AppUser
    public int UserId { get; set; }
    public AppUser ReservedBy { get; set; } = null!;
}
