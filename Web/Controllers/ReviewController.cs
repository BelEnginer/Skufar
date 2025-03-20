using Application.DTOs.PostDtos;
using Application.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpGet("{reviewId:guid}")]
    public async Task<IActionResult> GetReviewById(Guid reviewId, CancellationToken ct)
    {
        var result = await reviewService.GetReviewByIdAsync(reviewId, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }

    //[Authorize]
    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken ct)
    {
        var result = await reviewService.DeleteReviewAsync(reviewId, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : NoContent();
    }

    //[Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewPostDto reviewPostDto, CancellationToken ct)
    {
        var result = await reviewService.CreateReviewAsync(reviewPostDto, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Created("CreatedReview", result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetReviewsByReceiverId(Guid receiverId, CancellationToken ct)
    {
        var result = await reviewService.GetReviewsByReceiverIdAsync(receiverId, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }
}