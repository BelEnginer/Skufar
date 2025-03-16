namespace Application.DTOs.GetDtos;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public int Rate { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}