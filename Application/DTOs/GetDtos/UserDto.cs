namespace Application.DTOs.GetDtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Location { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; } 
}