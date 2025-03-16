using System.Text.Json.Serialization;

namespace Domain.Entites;

public class Review
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    [JsonIgnore] 
    public User Receiver { get; set; }
    [JsonIgnore] 
    public User Sender { get; set; }
    public int Rate { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}