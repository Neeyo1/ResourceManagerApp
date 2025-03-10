using API.DTOs.Reservation;

namespace API.DTOs.Room;

public class RoomWithReservationsDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public required string RoomType { get; set; }
    public required string RoomStatus { get; set; }
    public IEnumerable<RoomReservationDto> RoomReservations { get; set; } = [];
}
