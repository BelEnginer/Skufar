
using Application.Abstractions.IRepositories;
using Domain.Entites;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository(ApplicationDbContext context) : BaseRepository<Review>(context), IReviewRepository
{
    private static IQueryable<Review> IncludeAllRelations(IQueryable<Review> query)
    {
        return query
            .Include(s => s.Sender)
            .Include(r => r.Receiver);
    }
    
    public async Task<Review?> GetReviewByIdAsync(Guid id,CancellationToken ct) => 
        await IncludeAllRelations(GetByFilter(i => i.Id == id))
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

    public async Task<List<Review?>> GetReviewsByReceiverIdAsync(Guid receiverId,CancellationToken ct) => 
        (await IncludeAllRelations(GetByFilter(i => i.ReceiverId == receiverId))
            .AsNoTracking()
            .ToListAsync(ct))!;

    public async Task CreateReviewAsync(Review review,CancellationToken ct)
    {
        review.Id = Guid.NewGuid();
        review.CreatedAt = DateTime.UtcNow;
        await Context.Rewievs.AddAsync(review,ct);
        await Context.SaveChangesAsync(ct);
    }
    
    public void DeleteReview(Review review)
    {
        Delete(review);
        Context.SaveChanges();
    }
}