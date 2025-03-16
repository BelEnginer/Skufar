using System.Text.Json.Serialization;
using Domain.Enums;

namespace Application.DTOs.PostDtos;

public class TradeRequestPostDto
{
    public Guid ItemOfferedId { get; set; }
    public Guid ItemRequestedId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}