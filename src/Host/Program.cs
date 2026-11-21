using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Ginecologia.Modules.Auth.Api;
using Ginecologia.Modules.Auth.Infrastructure;
using Ginecologia.Modules.Citas.Api;
using Ginecologia.Modules.Citas.Infrastructure;
using Ginecologia.Modules.Facturacion.Api;
using Ginecologia.Modules.Facturacion.Infrastructure;
using Ginecologia.Modules.Inventario.Api;
using Ginecologia.Modules.Inventario.Infrastructure;
using Ginecologia.Modules.HistoriaClinica.Api;
using Ginecologia.Modules.HistoriaClinica.Infrastructure;
using Ginecologia.Modules.LaboratorioExterno.Api;
using Ginecologia.Modules.LaboratorioExterno.Infrastructure;
using Ginecologia.Modules.Notificaciones.Api;
using Ginecologia.Modules.Notificaciones.Infrastructure;
using Ginecologia.Modules.Pacientes.Api;
using Ginecologia.Modules.Pacientes.Infrastructure;
using Ginecologia.Modules.Prospeccion.Api;
using Ginecologia.Modules.Prospeccion.Infrastructure;
using Ginecologia.Modules.Promociones.Api;
using Ginecologia.Modules.Promociones.Infrastructure;
using Ginecologia.Modules.Usuarios.Api;
using Ginecologia.Modules.Usuarios.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
var smtpPort = int.TryParse(builder.Configuration["SMTP_PORT"], out var configuredSmtpPort) ? configuredSmtpPort : 587;
var smtpUseStartTls = !bool.TryParse(builder.Configuration["SMTP_USE_STARTTLS"], out var configuredUseStartTls) || configuredUseStartTls;
var smtpOptions = new SmtpOptions(
    builder.Configuration["SMTP_HOST"],
    smtpPort,
    builder.Configuration["SMTP_USERNAME"],
    builder.Configuration["SMTP_PASSWORD"],
    builder.Configuration["SMTP_FROM_EMAIL"],
    builder.Configuration["SMTP_FROM_NAME"],
    smtpUseStartTls);
builder.Services.AddAuthModule(connectionString, jwtOptions);
builder.Services.AddUsuariosModule(connectionString);
builder.Services.AddPacientesModule(connectionString);
builder.Services.AddProspeccionModule(connectionString);
builder.Services.AddCitasModule(connectionString);
builder.Services.AddHistoriaClinicaModule(connectionString);
builder.Services.AddLaboratorioExternoModule(connectionString);
builder.Services.AddFacturacionModule(connectionString);
builder.Services.AddInventarioModule(connectionString);
builder.Services.AddNotificacionesModule(connectionString, smtpOptions);
builder.Services.AddPromocionesModule(connectionString);
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ginecología API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegá el JWT obtenido de /api/auth/login (sin el prefijo \"Bearer \").",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() },
    });
});
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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireClinicalStaff", policy => policy.RequireRole("Admin", "Ginecologa"));
    options.AddPolicy("RequireBillingStaff", policy => policy.RequireRole("Admin", "Recepcionista"));
    options.AddPolicy("RequireInventoryStaff", policy => policy.RequireRole("Admin", "Recepcionista"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await dbContext.Database.MigrateAsync();
    var usuariosDbContext = scope.ServiceProvider.GetRequiredService<UsuariosDbContext>();
    await usuariosDbContext.Database.MigrateAsync();
    var pacientesDbContext = scope.ServiceProvider.GetRequiredService<PacientesDbContext>();
    await pacientesDbContext.Database.MigrateAsync();
    var prospeccionDbContext = scope.ServiceProvider.GetRequiredService<ProspeccionDbContext>();
    await prospeccionDbContext.Database.MigrateAsync();
    var citasDbContext = scope.ServiceProvider.GetRequiredService<CitasDbContext>();
    await citasDbContext.Database.MigrateAsync();
    var historiaClinicaDbContext = scope.ServiceProvider.GetRequiredService<HistoriaClinicaDbContext>();
    await historiaClinicaDbContext.Database.MigrateAsync();
    var laboratorioExternoDbContext = scope.ServiceProvider.GetRequiredService<LaboratorioExternoDbContext>();
    await laboratorioExternoDbContext.Database.MigrateAsync();
    var facturacionDbContext = scope.ServiceProvider.GetRequiredService<FacturacionDbContext>();
    await facturacionDbContext.Database.MigrateAsync();
    var inventarioDbContext = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
    await inventarioDbContext.Database.MigrateAsync();
    var notificacionesDbContext = scope.ServiceProvider.GetRequiredService<NotificacionesDbContext>();
    await notificacionesDbContext.Database.MigrateAsync();
    var promocionesDbContext = scope.ServiceProvider.GetRequiredService<PromocionesDbContext>();
    await promocionesDbContext.Database.MigrateAsync();
}
await AuthSeed.SeedAdminAsync(
    app.Services,
    app.Configuration["Auth:SeedAdminEmail"],
    app.Configuration["AUTH_SEED_ADMIN_PASSWORD"] ?? app.Configuration["Auth:SeedAdminPassword"]);

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapAuthEndpoints();
app.MapUsuariosEndpoints();
app.MapPacientesEndpoints();
app.MapProspeccionEndpoints();
app.MapCitasEndpoints();
app.MapHistoriaClinicaEndpoints();
app.MapLaboratorioExternoEndpoints();
app.MapFacturacionEndpoints();
app.MapInventoryEndpoints();
app.MapNotificationEndpoints();
app.MapPromotionEndpoints();

app.Run();
