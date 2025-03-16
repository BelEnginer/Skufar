using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PostDtos;

public class UserRegisterDto
{   [Required(ErrorMessage = "Please Enter Username")]
    [MaxLength(70, ErrorMessage = "Maximum length for the Type is 70 characters.")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }
    public required string Password { get; set; }
    [Required(ErrorMessage = "Location is required")]
    public string? Location { get; set; }
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone format.")]
    public string? Phone { get; set; }
}