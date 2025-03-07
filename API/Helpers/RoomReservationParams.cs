namespace API.Helpers;

public class RoomReservationParams : PaginationParams
{
    public DateTime? ReservedFrom { get; set; }
    public DateTime? ReservedTo { get; set; }
    public int? RoomId { get; set; }
    public int? UserId { get; set; }
    public string Status { get; set; } = "all";
    public string OrderBy { get; set; } = "name";
}
