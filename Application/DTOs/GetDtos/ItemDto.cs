using System.Text.Json.Serialization;
using Domain.Enums;

namespace Application.DTOs.GetDtos;

public class ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid OwnerId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Condition Condition { get; set; }
    public string? PreviewImagePath { get; set; }
    public List<string>? ImagePaths { get; set; } 
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}
