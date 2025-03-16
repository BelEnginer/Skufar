using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PostDtos;

public class ReviewPostDto
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    [Required]
    public int Rate { get; set; }
    public string? Comment { get; set; }
}