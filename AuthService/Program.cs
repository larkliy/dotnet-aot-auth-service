using AuthService;
using AuthService.Endpoints;
using AuthService.ExceptionHandlers;
using AuthService.Options;
using AuthService.Repositories;
using AuthService.Repositories.Abstractions;
using AuthService.Services;
using AuthService.Services.Abstractions;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System.Data;

#if DEBUG
using Scalar.AspNetCore;
#endif

[module: DapperAot]

var builder = WebApplication.CreateSlimBuilder(args);

#if DEBUG
builder.Services.AddOpenApi();
#endif

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=users.db";

builder.Services.AddScoped<IDbConnection>(_ =>
{
    var connBuilder = new SqliteConnectionStringBuilder(connectionString);
    var dbPath = connBuilder.DataSource;

    if (!string.IsNullOrEmpty(dbPath) && dbPath != ":memory:")
    {
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    var conn = new SqliteConnection(connectionString);
    conn.Open();
    return conn;
});

builder.Services.AddScoped<IAuthRepository, UserRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDatabaseInitializer, SqliteDatabaseInitializer>();

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<SqliteUniqueViolationExceptionHandler>();

builder.Services.AddValidation();

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                 ?? throw new InvalidOperationException("JWT configuration section is missing");

builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();

builder.Services.AddSingleton<IJwtService, JwtService>();

builder.Services.ConfigureHttpJsonOptions(options
    => options.SerializerOptions.TypeInfoResolverChain.Insert(0, SerializationContext.Default));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtValidationParametersFactory.Create(jwtOptions);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var dbInit = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await dbInit.InitializeAsync();
}

#if DEBUG
if (app.Environment.IsDevelopment()) 
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}
#endif

app.MapAuthEndpoints();
app.MapAdminEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();