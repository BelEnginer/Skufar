namespace Application.DTOs.PostDtos;

public class MessagePostDto
{
    public Guid? ChatId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
}
