using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos.Admin;

public record AdminUpdateUserRequest(
    [EmailAddress, StringLength(300, MinimumLength = 5)] 
    string Email,
    string Role
);
