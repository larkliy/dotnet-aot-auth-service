using AuthService;
using AuthService.Endpoints;
using AuthService.Options;
using AuthService.Repositories;
using AuthService.Services;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

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
    var conn = new SqliteConnection(connectionString);
    conn.Open();
    return conn;
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDatabaseInitializer, SqliteDatabaseInitializer>();

builder.Services.AddValidation();

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
        var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

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