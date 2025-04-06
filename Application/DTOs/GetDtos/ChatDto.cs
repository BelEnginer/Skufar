namespace Application.DTOs.GetDtos;

public class ChatDto
{
    public Guid Id { get; set; }
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public string? LastMessage { get; set; }
    public DateTime LastActivity { get; set; }
}