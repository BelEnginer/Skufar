using Application.DTOs.PostDtos;
using Application.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpGet("{reviewId:guid}")]
    public async Task<ActionResult> GetReviewById(Guid reviewId,CancellationToken ct)
    {
        var result = await reviewService.GetReviewByIdAsync(reviewId,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Ok(result.Value);
    }
    //[Authorize]
    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken ct)
    {
        var result = await reviewService.DeleteReviewAsync(reviewId,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")

            };
        }
        return NoContent();
    }
    //[Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewPostDto reviewPostDto,CancellationToken ct)
    {
        var result = await reviewService.CreateReviewAsync(reviewPostDto,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Created("CreatedReview", result.Value);
    }
    [HttpGet]
    public async Task<ActionResult> GetReviewsByReceiverId(Guid receiverId, CancellationToken ct)
    {
        var result = await reviewService.GetReviewsByReceiverIdAsync(receiverId,ct);
        return Ok(result.Value);
    }
}