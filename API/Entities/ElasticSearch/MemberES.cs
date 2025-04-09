using Nest;

namespace API.Entities.ElasticSearch;

public class MemberES
{
    [Keyword(Name = "id")]
    public required string Id { get; set; }

    [Keyword(Name = "firstName")]
    public required string FirstName { get; set; }

    [Keyword(Name = "lastName")]
    public required string LastName { get; set; }
}
