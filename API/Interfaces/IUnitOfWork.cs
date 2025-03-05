namespace API.Interfaces;

public interface IUnitOfWork
{
    ITokenRepository TokenRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}
