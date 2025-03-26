using System.ComponentModel.DataAnnotations;

namespace client.DTOs.Reservation.Room;

public class RoomReservationCreateDto
{
    [Required]
    public DateTime ReservedFrom { get; set; }

    [Required]
    public string? ReservedFromString { get; set; }

    [Required]
    public DateTime ReservedTo { get; set; }

    [Required]
    public string? ReservedToString { get; set; }

    [Required]
    public int RoomId { get; set; }
}
