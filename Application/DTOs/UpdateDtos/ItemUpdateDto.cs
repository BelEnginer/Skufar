using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Application.DTOs.UpdateDtos;

public class ItemUpdateDto
{   [MaxLength(50, ErrorMessage = "Maximum length for the Type is 50 characters.")]
    public string? Name { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Condition Condition { get; set; }
    public string? Description { get; set; }
    public bool? IsAvailable { get; set; }
    public string? ImagePath { get; set; }
}