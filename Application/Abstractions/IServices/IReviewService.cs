using Application.Common;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;

namespace Application.IServices;

public interface IReviewService
{
    Task<Result<List<ReviewDto>>> GetReviewsByReceiverIdAsync(Guid receiverId,CancellationToken ct);
    Task<Result<ReviewDto>> GetReviewByIdAsync(Guid reviewId,CancellationToken ct);
    Task<Result<ReviewDto>> CreateReviewAsync(ReviewPostDto reviewPostDto,CancellationToken ct);
    Task<Result<Unit>> DeleteReviewAsync(Guid id, CancellationToken ct);
}