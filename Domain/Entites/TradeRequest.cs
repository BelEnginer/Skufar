using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Entites;

public class TradeRequest
{
    public Guid Id { get; set; }
    public Guid ItemOfferedId { get; set; }
    public Guid ItemRequestedId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Status Status { get; set; }
    [JsonIgnore] 
    public Item Item { get; set; }
    [JsonIgnore] 
    public User Sender { get; set; }
    [JsonIgnore] 
    public User Receiver { get; set; }
    public DateTime CreatedAt { get; set; }
}