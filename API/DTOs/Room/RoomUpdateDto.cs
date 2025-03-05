using API.Entities;

namespace API.DTOs.Room;

public class RoomUpdateDto
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? RoomType { get; set; }
}
