using Application.Abstractions.IServices;
using Application.DTOs.PostDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService _reviewService) : ControllerBase
{
    [HttpGet("{reviewId:guid}")]
    public async Task<IActionResult> GetReviewById(Guid reviewId, CancellationToken ct)
    {
        var result = await _reviewService.GetReviewByIdAsync(reviewId, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken ct)
    {
        var result = await _reviewService.DeleteReviewAsync(reviewId, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewPostDto reviewPostDto, CancellationToken ct)
    {
        var result = await _reviewService.CreateReviewAsync(reviewPostDto, ct);
        return result.IsSuccess ? Created("CreatedReview", result.Value): BadRequest(result.ErrorMessage);
    }

    [HttpGet]
    public async Task<IActionResult> GetReviewsByReceiverId(Guid receiverId, CancellationToken ct)
    {
        var result = await _reviewService.GetReviewsByReceiverIdAsync(receiverId, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }
}