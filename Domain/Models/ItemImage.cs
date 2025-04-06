using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class ItemImage
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        [JsonIgnore] 
        public Item Item { get; set; }
        public string ImagePath { get; set; }
    }
}