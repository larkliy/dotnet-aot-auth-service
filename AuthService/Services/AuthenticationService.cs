using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Common;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Options;
using AuthService.Repositories.Abstractions;
using AuthService.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace AuthService.Services;

public sealed class AuthenticationService(
    IAuthRepository repository,
    IJwtService jwtService,
    IPasswordHasher passwordHasher,
    IOptions<JwtOptions> options) : IAuthenticationService
{
    private const string DefaultRole = "User";

    private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(options.Value.RefreshTokenExpiryDays);

    public Task RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        return repository.CreateUserAsync(email, passwordHasher.Hash(password), DefaultRole, cancellationToken);
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await repository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !passwordHasher.Verify(password, user.PasswordHash))
            return Result<AuthResponse>.Fail(ServiceFailure.InvalidCredentials);

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(string accessToken, string refreshToken, CancellationToken cancellationToken)
    {
        var principal = jwtService.GetPrincipalFromExpiredToken(accessToken);
        if (principal is null)
            return Result<AuthResponse>.Fail(ServiceFailure.InvalidCredentials);

        var email = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value
                 ?? principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var user = email is null ? null : await repository.GetByEmailAsync(email, cancellationToken);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Result<AuthResponse>.Fail(ServiceFailure.InvalidCredentials);

        return await IssueTokensAsync(user, cancellationToken);
    }

    private async Task<Result<AuthResponse>> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = jwtService.GenerateToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();

        await repository.UpdateRefreshTokenAsync(
            user.Id,
            refreshToken,
            DateTime.UtcNow + _refreshTokenLifetime,
            cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken));
    }
}