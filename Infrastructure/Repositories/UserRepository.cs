using Application.Abstractions.IRepositories;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
{
    private static IQueryable<User> IncludeAllRelations(IQueryable<User> query)
    {
        return query
            .Include(t => t.TradeRequests)
            .Include(r => r.Reviews)
            .Include(i => i.Items);
    }

    public async Task<User?> GetUserByIdAsync(Guid? userId,CancellationToken ct) => 
        await IncludeAllRelations(GetByFilter(i => i.Id == userId))
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

    public async Task<User?> GetUserByEmailAsync(string? email,CancellationToken ct) =>
        await IncludeAllRelations(GetByFilter(i => i.Email == email))
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    
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