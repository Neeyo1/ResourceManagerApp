namespace API.Interfaces;

public interface IUnitOfWork
{
    ITokenRepository TokenRepository { get; }
    IRoomRepository RoomRepository { get; }
    IReservationRepository ReservationRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}
