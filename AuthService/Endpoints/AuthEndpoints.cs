using AuthService.Dtos;
using AuthService.Repositories;
using AuthService.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var usersGroup = endpoints.MapGroup("auth").WithTags("Auth");

        usersGroup.MapPost("/register", RegisterUserAsync);
        usersGroup.MapPost("/login", LoginUserAsync);
        usersGroup.MapPost("/refresh", RefreshUserTokenAsync);
    }

    private static async Task<IResult> RegisterUserAsync(AuthRequest request, IUserRepository repository)
    {
        if (await repository.ExistsByEmailAsync(request.Email))
            return Results.Conflict("A user with this email already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        await repository.CreateUserAsync(request.Email, passwordHash, DateTime.UtcNow);

        return Results.Ok();
    }

    private static async Task<IResult> LoginUserAsync(
        AuthRequest request,
        IUserRepository repository,
        IJwtService jwtService)
    {
        var user = await repository.GetByEmailAsync(request.Email);

        if (user == null)
            return Results.BadRequest("A user not exists. Please, register first.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Results.Unauthorized();

        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await repository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryTime);

        var accessToken = jwtService.GenerateToken(user);

        return Results.Ok(new AuthResponse(accessToken, refreshToken));
    }

    private static async Task<IResult> RefreshUserTokenAsync(
            RefreshTokenRequest request,
            IUserRepository repository,
            IJwtService jwtService)
    {
        var principal = jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Results.Unauthorized();

        var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)
                      ?? principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (emailClaim == null)
            return Results.Unauthorized();

        var user = await repository.GetByEmailAsync(emailClaim.Value);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Results.Unauthorized();

        var newAccessToken = jwtService.GenerateToken(user);
        var newRefreshToken = jwtService.GenerateRefreshToken();
        var newExpiryTime = DateTime.UtcNow.AddDays(7);

        await repository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, newExpiryTime);

        return Results.Ok(new AuthResponse(newAccessToken, newRefreshToken));
    }
}