using AuthService.Models;
using System.Security.Claims;

namespace AuthService.Services.Abstractions;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
