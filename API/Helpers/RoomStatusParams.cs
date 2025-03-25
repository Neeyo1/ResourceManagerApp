namespace API.Helpers;

public class RoomStatusParams : RoomParams
{
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public string Status { get; set; } = "all";
}
