using Domain.Models;

namespace Application.Abstractions.IRepositories;

public interface IReviewRepository
{
    Task<Review?> GetReviewByIdAsync(Guid reviewerId,CancellationToken ct);
    Task<List<Review>> GetReviewsByReceiverIdAsync(Guid receiverId, CancellationToken ct);
    Task CreateReviewAsync(Review review, CancellationToken ct);
    Task DeleteReviewAsync(Review review,CancellationToken ct);
}