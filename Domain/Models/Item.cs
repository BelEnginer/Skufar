using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [JsonIgnore] 
        public Category Category { get; set; }
        public string? PreviewImagePath { get; set; }
    
        public Condition Condition { get; set; }
        [JsonIgnore] 
        public ICollection<TradeRequest>? Requests { get; set; }
        [JsonIgnore] 
        public ICollection<ItemImage> Images { get; set; } = new List<ItemImage>();
        [JsonIgnore] 
        public User Owner { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}