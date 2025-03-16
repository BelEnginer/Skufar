namespace Application.DTOs.GetDtos;

public class TradeRequestDto
{
    public Guid Id { get; set; }
    public Guid ItemOfferedId { get; set; }
    public Guid ItemRequestedId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public DateTime CreatedAt { get; set; }
}