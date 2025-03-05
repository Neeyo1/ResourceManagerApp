namespace API.DTOs.Room;

public class RoomDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public required string RoomType { get; set; }
}
