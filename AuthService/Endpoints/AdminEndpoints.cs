using AuthService.Dtos.Admin;
using AuthService.Repositories;

namespace AuthService.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var adminGroup = endpoints.MapGroup("admin/users")
            .WithTags("Admin")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        adminGroup.MapGet("/", GetAllUsersAsync);
        adminGroup.MapGet("/{id:int}", GetUserByIdAsync);
        adminGroup.MapPost("/", CreateUserAsync);
        adminGroup.MapPut("/{id:int}", UpdateUserAsync);
        adminGroup.MapDelete("/{id:int}", DeleteUserAsync);
    }

    private static async Task<IResult> GetAllUsersAsync(IUserRepository repository)
    {
        var users = await repository.GetAllUsersAsync();
        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserByIdAsync(int id, IUserRepository repository)
    {
        var user = await repository.GetByIdAsync(id);
        return user is not null ? Results.Ok(user) : Results.NotFound();
    }

    private static async Task<IResult> CreateUserAsync(AdminCreateUserRequest request, IUserRepository repository)
    {
        if (await repository.ExistsByEmailAsync(request.Email))
            return Results.Conflict("User with this email already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await repository.AdminCreateUserAsync(
            request.Email,
            passwordHash,
            request.Role,
            DateTime.UtcNow);

        return Results.Created();
    }

    private static async Task<IResult> UpdateUserAsync(int id, AdminUpdateUserRequest request, IUserRepository repository)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null)
            return Results.NotFound();

        if (user.Email != request.Email && await repository.ExistsByEmailAsync(request.Email))
            return Results.Conflict("Email is already taken by another user.");

        await repository.UpdateUserAsync(id, request.Email, request.Role);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUserAsync(int id, IUserRepository repository)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null)
            return Results.NotFound();

        await repository.DeleteUserAsync(id);
        return Results.NoContent();
    }
}