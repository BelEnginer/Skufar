using Domain.Entites;

namespace Application.Abstractions.IRepositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid? userId, CancellationToken ct); 
    Task<User?> GetUserByEmailAsync(string? email, CancellationToken ct);
    Task CreateUserAsync(User user, CancellationToken ct);
    Task UpdateUserAsync(User user, CancellationToken ct);
}