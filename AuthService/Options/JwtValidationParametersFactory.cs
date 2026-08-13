using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Options;

public static class JwtValidationParametersFactory
{
    public static TokenValidationParameters Create(JwtOptions options, bool validateLifetime = true) => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = validateLifetime,
        ValidateIssuerSigningKey = true,
        ValidIssuer = options.Issuer,
        ValidAudience = options.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key))
    };
}