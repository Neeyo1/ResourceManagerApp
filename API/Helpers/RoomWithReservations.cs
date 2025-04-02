using shared.DTOs.Reservation;
using API.Entities;

namespace API.Helpers;

public class RoomWithReservations
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public required RoomType RoomType { get; set; }
    public required RoomStatus RoomStatus { get; set; }
    public IEnumerable<RoomReservationDto> RoomReservations { get; set; } = [];
}
