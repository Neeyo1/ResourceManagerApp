using System.ComponentModel.DataAnnotations;

namespace client.DTOs.Room;

public class RoomCreateDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int Capacity { get; set; }

    [Required]
    public required string RoomType { get; set; }
}
