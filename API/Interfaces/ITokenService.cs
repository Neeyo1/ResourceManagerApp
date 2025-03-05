using System.Security.Claims;
using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
    RefreshToken CreateRefreshToken(string email);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
