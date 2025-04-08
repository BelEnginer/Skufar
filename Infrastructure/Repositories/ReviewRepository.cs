
using Application.Abstractions.IRepositories;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class ReviewRepository(ApplicationDbContext context) : BaseRepository<Review>(context), IReviewRepository
{
    public async Task<Review?> GetReviewByIdAsync(Guid id, CancellationToken ct) =>
        await Query
            .Include(r => r.Receiver)
            .Include(r => r.Sender)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id,ct);

    public async Task<List<Review>> GetReviewsByReceiverIdAsync(Guid receiverId, CancellationToken ct) =>
        await Query
            .Include(r => r.Receiver)
            .Include(r => r.Sender)
            .Where(i => i.ReceiverId == receiverId)
            .AsNoTracking()
            .ToListAsync(ct);
    public async Task CreateReviewAsync(Review review,CancellationToken ct)
    {
        review.Id = Guid.NewGuid();
        review.CreatedAt = DateTime.UtcNow;
        await Context.Rewievs.AddAsync(review,ct);
        await Context.SaveChangesAsync(ct);
    }
    
    public async Task DeleteReviewAsync(Review review,CancellationToken ct)
    {
        Delete(review);
        await context.SaveChangesAsync(ct);
    }
}