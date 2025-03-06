using API.Interfaces;

namespace API.Data;

public class UnitOfWork(DataContext context, ITokenRepository tokenRepository,
    IRoomRepository roomRepository, IReservationRepository reservationRepository) : IUnitOfWork
{
    public ITokenRepository TokenRepository => tokenRepository;
    public IRoomRepository RoomRepository => roomRepository;
    public IReservationRepository ReservationRepository => reservationRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
