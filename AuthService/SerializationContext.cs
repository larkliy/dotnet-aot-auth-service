using AuthService.Dtos;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AuthService;

[JsonSerializable(typeof(RefreshTokenResponse))]
[JsonSerializable(typeof(RefreshTokenRequest))]
[JsonSerializable(typeof(AuthResponse))]
[JsonSerializable(typeof(AuthRequest))]
[JsonSerializable(typeof(User))]

[JsonSerializable(typeof(HttpValidationProblemDetails))]
[JsonSerializable(typeof(ProblemDetails))]

[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class SerializationContext : JsonSerializerContext
{

}
