using System.Text.Json.Serialization;

namespace AuthService.Models;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }

    [JsonIgnore]
    public required string PasswordHash { get; set; }

    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}