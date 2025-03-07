namespace API.Helpers;

public class RoomParams : PaginationParams
{
    public string? Name { get; set; }
    public int? MinCapacity { get; set; }
    public int? MaxCapacity { get; set; }
    public string Type { get; set; } = "all";
    public string OrderBy { get; set; } = "name";
}