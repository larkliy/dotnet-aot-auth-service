using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos;

public record AuthRequest(
    [EmailAddress, StringLength(300, MinimumLength = 5)]
    string Email,
    [StringLength(30, MinimumLength = 8)]
    string Password
);