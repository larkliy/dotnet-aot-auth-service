using AuthService.Dtos;
using AuthService.Dtos.Admin;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AuthService;


[JsonSerializable(typeof(AdminCreateUserRequest))]
[JsonSerializable(typeof(AdminUpdateUserRequest))]
[JsonSerializable(typeof(IEnumerable<User>))]

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
