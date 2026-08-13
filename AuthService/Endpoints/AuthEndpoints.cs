using AuthService.Dtos;
using AuthService.Services.Abstractions;

namespace AuthService.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var usersGroup = endpoints.MapGroup("auth").WithTags("Auth");

        usersGroup.MapPost("/register", RegisterAsync);
        usersGroup.MapPost("/login", LoginAsync);
        usersGroup.MapPost("/refresh", RefreshAsync);
    }

    private static async Task<IResult> RegisterAsync(
        AuthRequest request,
        IAuthenticationService authService,
        CancellationToken cancellationToken)
    {
        await authService.RegisterAsync(request.Email, request.Password, cancellationToken);
        return Results.Ok();
    }

    private static async Task<IResult> LoginAsync(
        AuthRequest request,
        IAuthenticationService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request.Email, request.Password, cancellationToken);
        return Results.Ok(response);
    }

    private static async Task<IResult> RefreshAsync(
        RefreshTokenRequest request,
        IAuthenticationService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.RefreshAsync(request.AccessToken, request.RefreshToken, cancellationToken);
        return Results.Ok(response);
    }
}