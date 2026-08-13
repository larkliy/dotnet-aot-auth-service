using AuthService.Dtos.Admin;
using AuthService.Services.Abstractions;

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

    private static async Task<IResult> GetAllUsersAsync(IAdminUserService adminService, CancellationToken cancellationToken)
    {
        var users = await adminService.GetAllAsync(cancellationToken);
        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserByIdAsync(
        int id,
        IAdminUserService adminService,
        CancellationToken cancellationToken)
    {
        var result = await adminService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound();
    }

    private static async Task<IResult> CreateUserAsync(
        AdminCreateUserRequest request,
        IAdminUserService adminService,
        CancellationToken cancellationToken)
    {
        await adminService.CreateAsync(request, cancellationToken);
        return Results.Created();
    }

    private static async Task<IResult> UpdateUserAsync(
        int id,
        AdminUpdateUserRequest request,
        IAdminUserService adminService,
        CancellationToken cancellationToken)
    {
        var result = await adminService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> DeleteUserAsync(
        int id,
        IAdminUserService adminService,
        CancellationToken cancellationToken)
    {
        var result = await adminService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Results.NotFound();
    }
}