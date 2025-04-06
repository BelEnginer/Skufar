using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        [JsonIgnore] 
        public ICollection<TradeRequest>? TradeRequests { get; set; }
        [JsonIgnore] 
        public ICollection<Review>? Reviews { get; set; }
        [JsonIgnore] 
        public ICollection<Item>? Items { get; set; }
        [JsonIgnore]
        public ICollection<Chat>? Chats { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}