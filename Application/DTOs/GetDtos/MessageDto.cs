namespace Application.DTOs.GetDtos;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public bool IsRead { get; set; }
}