using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Reservation;

public class RoomReservationCreateDto
{
    [Required]
    public required DateTime ReservedFrom { get; set; }

    [Required]
    public required DateTime ReservedTo { get; set; }

    [Required]
    public required int RoomId { get; set; }
}
