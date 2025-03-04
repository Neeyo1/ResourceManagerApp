namespace API.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public RoomType RoomType { get; set; }

    //Room - RoomReservation
    public ICollection<RoomReservation> RoomReservations { get; set; } = [];
}
