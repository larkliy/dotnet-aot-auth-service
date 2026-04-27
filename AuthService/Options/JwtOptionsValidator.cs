using Microsoft.Extensions.Options;

namespace AuthService.Options;

[OptionsValidator]
public partial class JwtOptionsValidator : IValidateOptions<JwtOptions>;
