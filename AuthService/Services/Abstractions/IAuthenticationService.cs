using AuthService.Dtos;

namespace AuthService.Services.Abstractions;

public interface IAuthenticationService
{
    Task RegisterAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
}