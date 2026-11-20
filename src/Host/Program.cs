using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Ginecologia.Modules.Auth.Api;
using Ginecologia.Modules.Auth.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var connectionString = builder.Configuration.GetConnectionString("Auth")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure ConnectionStrings__Auth o ConnectionStrings__DefaultConnection.");
var signingKey = builder.Configuration["JWT_SIGNING_KEY"] ?? builder.Configuration["Auth:JwtSigningKey"];
if (string.IsNullOrWhiteSpace(signingKey) || signingKey.Length < 32)
    throw new InvalidOperationException("JWT_SIGNING_KEY debe contener al menos 32 caracteres.");

var jwtOptions = new JwtOptions(signingKey);
builder.Services.AddAuthModule(connectionString, jwtOptions);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            NameClaimType = JwtRegisteredClaimNames.Email,
            RoleClaimType = "role",
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await dbContext.Database.MigrateAsync();
}
await AuthSeed.SeedAdminAsync(
    app.Services,
    app.Configuration["Auth:SeedAdminEmail"],
    app.Configuration["AUTH_SEED_ADMIN_PASSWORD"] ?? app.Configuration["Auth:SeedAdminPassword"]);

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapAuthEndpoints();

app.Run();
