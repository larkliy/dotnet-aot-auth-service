using AuthService.Common.Exceptions;
using AuthService.Dtos.Admin;
using AuthService.Models;
using AuthService.Repositories.Abstractions;
using AuthService.Services.Abstractions;

namespace AuthService.Services;

public sealed class AdminUserService(IUserRepository repository, IPasswordHasher passwordHasher) : IAdminUserService
{
    public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        => repository.GetAllUsersAsync(cancellationToken);

    public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await repository.GetByIdAsync(id, cancellationToken) ?? throw new UserNotFoundException();

    public Task CreateAsync(AdminCreateUserRequest request, CancellationToken cancellationToken = default)
        => repository.CreateUserAsync(request.Email, passwordHasher.Hash(request.Password), request.Role, cancellationToken);

    public async Task UpdateAsync(int id, AdminUpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.GetByIdAsync(id, cancellationToken) is null)
            throw new UserNotFoundException();

        await repository.UpdateUserAsync(id, request.Email, request.Role, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (await repository.GetByIdAsync(id, cancellationToken) is null)
            throw new UserNotFoundException();

        await repository.DeleteUserAsync(id, cancellationToken);
    }
}