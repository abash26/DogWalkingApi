using System.ComponentModel.DataAnnotations;

namespace DogWalkingApi.DTOs;

public class RegisterDTO
{
    [Required]
    public string Password { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Name { get; set; }
    public string Role { get; set; } = "owner";
}
