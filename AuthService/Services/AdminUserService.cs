using AuthService.Common;
using AuthService.Dtos.Admin;
using AuthService.Models;
using AuthService.Repositories.Abstractions;
using AuthService.Services.Abstractions;

namespace AuthService.Services;

public sealed class AdminUserService(IUserRepository repository, IPasswordHasher passwordHasher) : IAdminUserService
{
    public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        => repository.GetAllUsersAsync(cancellationToken);

    public async Task<Result<User>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken);
        return user is null
            ? Result<User>.Fail(ServiceFailure.NotFound)
            : Result<User>.Success(user);
    }

    public Task CreateAsync(AdminCreateUserRequest request, CancellationToken cancellationToken = default)
    {
        return repository.CreateUserAsync(request.Email, passwordHasher.Hash(request.Password), request.Role, cancellationToken);
    }

    public async Task<Result> UpdateAsync(int id, AdminUpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.GetByIdAsync(id, cancellationToken) is null)
            return Result.Fail(ServiceFailure.NotFound);

        await repository.UpdateUserAsync(id, request.Email, request.Role, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (await repository.GetByIdAsync(id, cancellationToken) is null)
            return Result.Fail(ServiceFailure.NotFound);

        await repository.DeleteUserAsync(id, cancellationToken);
        return Result.Success();
    }
}