using Application.Abstractions.IRepositories;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetUserByIdAsync(Guid? userId,CancellationToken ct) => 
        await Query
            .Include(t => t.TradeRequests)
            .Include(r => r.Reviews)
            .Include(i => i.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == userId,ct);

    public async Task<User?> GetUserByEmailAsync(string? email,CancellationToken ct) =>
        await Query
            .Include(t => t.TradeRequests)
            .Include(r => r.Reviews)
            .Include(i => i.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Email == email,ct);
    
    public async Task CreateUserAsync(User user,CancellationToken ct)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        await Context.Users.AddAsync(user,ct);
        await Context.SaveChangesAsync(ct);
    }

    public async Task UpdateUserAsync(User user,CancellationToken ct)
    {
        await Context.SaveChangesAsync(ct);
    }
}