using Nest;

namespace API.Entities.ElasticSearch;

public class RoomReservationES
{
    [Keyword(Name = "id")]
    public required string Id { get; set; }

    [Date(Name = "reservedFrom")]
    public DateTime ReservedFrom { get; set; }

    [Date(Name = "reservedTo")]
    public DateTime ReservedTo { get; set; }

    [Nested(Name = "reservedBy")]
    public MemberES ReservedBy { get; set; } = null!;
}
