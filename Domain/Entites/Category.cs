using System.Text.Json.Serialization;

namespace Domain.Entites;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    [JsonIgnore] 
    public ICollection<Item>? Items { get; set; }
}