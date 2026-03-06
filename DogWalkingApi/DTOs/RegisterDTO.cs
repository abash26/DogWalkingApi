using DogWalkingApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DogWalkingApi.DTOs;

public class RegisterDTO
{
    [Required]
    public string Password { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; } = UserRole.Owner;
}
