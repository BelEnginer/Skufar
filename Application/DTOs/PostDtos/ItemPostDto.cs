using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.PostDtos;

public class ItemPostDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Maximum length for the Type is 50 characters.")]
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid OwnerId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Condition Condition { get; set; }
    [Required(ErrorMessage = "Image is required")]
    public ICollection<IFormFile> Images { get; set; } =new List<IFormFile>();
    public bool IsAvailable { get; set; }
}