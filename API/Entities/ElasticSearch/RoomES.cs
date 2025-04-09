using Nest;

namespace API.Entities.ElasticSearch;

public class RoomES
{
    [Keyword(Name = "id")]
    public required string Id { get; set; }

    [Text(Name = "name")]
    public required string Name { get; set; }

    [Number(NumberType.Integer, Name = "capacity")]
    public int Capacity { get; set; }

    [Keyword(Name = "roomType")]
    public required string RoomType { get; set; }
}
