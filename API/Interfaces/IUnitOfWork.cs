namespace API.Interfaces;

public interface IUnitOfWork
{
    ITokenRepository TokenRepository { get; }
    IRoomRepository RoomRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}
