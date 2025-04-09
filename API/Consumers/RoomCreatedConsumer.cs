using API.Entities.ElasticSearch;
using AutoMapper;
using MassTransit;
using Nest;
using shared.Contracts.Room;

namespace API.Consumers;

public class RoomCreatedConsumer(IMapper mapper, ILogger<RoomCreatedConsumer> logger,
    IElasticClient elasticClient) : IConsumer<RoomCreated>
{
    public async Task Consume(ConsumeContext<RoomCreated> context)
    {
        logger.LogInformation("------ Consuming RoomCreated: {id} ------", context.Message.Id);
        var room = mapper.Map<RoomES>(context.Message);
        var response = await elasticClient.IndexAsync(room, x => x
            .Index("rooms")
            .Id(room.Id)
        );
        if (!response.IsValid)
        {
            throw new Exception($"Indexing room failed: {response.ServerError}");
        }
    }
}
