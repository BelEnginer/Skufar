using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UpdateDtos;

    public class UserUpdateDto
    {
        [MaxLength(70, ErrorMessage = "Maximum length for the Type is 70 characters.")]
        public string? Name { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        public string? Location { get; set; }
        [Phone(ErrorMessage = "Invalid phone format.")]
        public string? Phone { get; set; }
    }