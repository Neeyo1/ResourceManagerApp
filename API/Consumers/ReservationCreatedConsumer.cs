using API.Entities.ElasticSearch;
using AutoMapper;
using MassTransit;
using Nest;
using shared.Contracts.Reservation;

namespace API.Consumers;

public class ReservationCreatedConsumer(IMapper mapper, ILogger<ReservationCreatedConsumer> logger,
    IElasticClient elasticClient) : IConsumer<ReservationCreated>
{
    public async Task Consume(ConsumeContext<ReservationCreated> context)
    {
        logger.LogInformation("------ Consuming ReservationCreated: {id} ------", context.Message.Id);
        var reservation = mapper.Map<RoomReservationES>(context.Message);
        var response = await elasticClient.IndexAsync(reservation, x => x
            .Index("room_reservations")
            .Id(reservation.Id)
        );
        if (!response.IsValid)
        {
            throw new Exception($"Indexing room reservation failed: {response.ServerError}");
        }
    }
}
