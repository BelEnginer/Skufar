using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
        public DateTime LastActivity { get; set; }
        [JsonIgnore]
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}