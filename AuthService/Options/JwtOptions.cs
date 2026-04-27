using System.ComponentModel.DataAnnotations;

namespace AuthService.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required, StringLength(500, MinimumLength = 32)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;
}
