using AuthService.Dtos.Admin;
using AuthService.Models;

namespace AuthService.Services.Abstractions;

public interface IAdminUserService
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(AdminCreateUserRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, AdminUpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}