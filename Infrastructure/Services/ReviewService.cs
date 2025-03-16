using Application.Abstractions.IUnitOfWork;
using Application.Common;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.IServices;
using AutoMapper;
using Domain.Entites;
using Domain.Enums;
using Infrastructure.Helpers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ReviewService(IUnitOfWork _repository, 
    IMapper _mapper,
    ILogger<ReviewService> _logger) : IReviewService
{
    public async Task<Result<List<ReviewDto>>> GetReviewsByReceiverIdAsync(Guid receiverId,CancellationToken ct)
    {
        var reviews = await _repository.ReviewRepository.GetReviewsByReceiverIdAsync(receiverId, ct);
        var count = reviews?.Count ?? 0;
        _logger.LogInformation("Retrieved {ReviewsCount} reviews for receiver ID: {ReceiverId}", count, receiverId);
        return Result<List<ReviewDto>>.Success(_mapper.Map<List<ReviewDto>>(reviews));
    }

    public async Task<Result<ReviewDto>> CreateReviewAsync(ReviewPostDto reviewPostDto,CancellationToken ct)
    {
        return await ErrorHandlingHelper.ExecuteAsync(async () =>
        {
            var receiver = await _repository.UserRepository.GetUserByIdAsync(reviewPostDto.ReceiverId, ct);
            var sender = await _repository.UserRepository.GetUserByIdAsync(reviewPostDto.SenderId, ct);
            if (sender is null)
            {
                _logger.LogWarning("Sender {SenderId} not found", reviewPostDto.SenderId);
                return Result<ReviewDto>.Failure("Sender unauthorized", ErrorType.Unauthorized);
            }

            if (receiver is null)
            {
                _logger.LogWarning("Receiver {ReceiverId} not found", reviewPostDto.ReceiverId);
                return Result<ReviewDto>.Failure("Receiver not found", ErrorType.NotFound);
            }

            var review = _mapper.Map<Review>(reviewPostDto);
            review.Receiver = receiver;
            review.Sender = sender;
            await _repository.ReviewRepository.CreateReviewAsync(review, ct);
            await _repository.SaveAsync();
            _logger.LogInformation("Created review {ReviewId}", review.Id);
            return Result<ReviewDto>.Success(_mapper.Map<ReviewDto>(review));
        },_logger);
    }

    public async Task<Result<ReviewDto>> GetReviewByIdAsync(Guid reviewId,CancellationToken ct)
    {
        var review = await _repository.ReviewRepository.GetReviewByIdAsync(reviewId,ct);
        if (review is null)
        {
            _logger.LogWarning("Review {ReviewId} not found", reviewId);
            return Result<ReviewDto>.Failure("Review not found", ErrorType.NotFound);
        }
        _logger.LogInformation("Retrieved review {ReviewId}", reviewId);
        return Result<ReviewDto>.Success(_mapper.Map<ReviewDto>(review));
    }
    
    public async Task<Result<Unit>> DeleteReviewAsync(Guid reviewId,CancellationToken ct)
    {
        return await ErrorHandlingHelper.ExecuteAsync(async () =>
        {
            var reviewToDelete = await _repository.ReviewRepository.GetReviewByIdAsync(reviewId, ct);
            if (reviewToDelete is null)
            {
                _logger.LogWarning("Review {ReviewId} not found", reviewId);
                return Result<Unit>.Failure("Review not found", ErrorType.NotFound);
            }

            _repository.ReviewRepository.DeleteReview(reviewToDelete);
            await _repository.SaveAsync();
            _logger.LogInformation("Deleted review {ReviewId}", reviewToDelete.Id);
            return Result<Unit>.Success(Unit.Value);
        }, _logger);
    }
}