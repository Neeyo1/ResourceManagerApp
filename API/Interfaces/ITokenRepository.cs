using API.Entities;

namespace API.Interfaces;

public interface ITokenRepository
{
    void AddRefreshToken(RefreshToken refreshToken);
    void RemoveRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshToken(string email, string token);
}
