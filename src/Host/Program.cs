using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Ginecologia.Modules.Auth.Api;
using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Auth.Infrastructure;
using Ginecologia.Modules.Autorizacion.Api;
using Ginecologia.Modules.Autorizacion.Infrastructure;
using Ginecologia.Modules.Citas.Api;
using Ginecologia.Modules.Citas.Infrastructure;
using Ginecologia.Modules.Facturacion.Api;
using Ginecologia.Modules.Facturacion.Infrastructure;
using Ginecologia.Modules.Inventario.Api;
using Ginecologia.Modules.Inventario.Infrastructure;
using Ginecologia.Modules.HistoriaClinica.Api;
using Ginecologia.Modules.HistoriaClinica.Infrastructure;
using Ginecologia.Modules.ConsultaMedica.Api;
using Ginecologia.Modules.ConsultaMedica.Infrastructure;
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
var resendOptions = new ResendOptions(
    builder.Configuration["RESEND_API_KEY"],
    builder.Configuration["RESEND_FROM_EMAIL"],
    builder.Configuration["RESEND_FROM_NAME"]);
var frontendOptions = new AuthFrontendOptions(builder.Configuration["FRONTEND_URL"] ?? "http://localhost:3000");
builder.Services.AddAuthModule(connectionString, jwtOptions, frontendOptions);
builder.Services.AddUsuariosModule(connectionString);
builder.Services.AddPacientesModule(connectionString);
builder.Services.AddProspeccionModule(connectionString);
builder.Services.AddCitasModule(connectionString);
builder.Services.AddHistoriaClinicaModule(connectionString);
builder.Services.AddConsultaMedicaModule(connectionString);
builder.Services.AddLaboratorioExternoModule(connectionString);
builder.Services.AddFacturacionModule(connectionString);
builder.Services.AddInventarioModule(connectionString);
builder.Services.AddNotificacionesModule(connectionString, resendOptions);
builder.Services.AddPromocionesModule(connectionString);
builder.Services.AddAutorizacionModule(connectionString);
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
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    context.RequestServices.GetRequiredService<ILogger<Program>>()
        .LogError(exception, "Error no controlado en {Path}", context.Request.Path);
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { message = "Ocurrió un error inesperado. Intenta de nuevo." });
}));

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
    var consultaMedicaDbContext = scope.ServiceProvider.GetRequiredService<ConsultaMedicaDbContext>();
    await consultaMedicaDbContext.Database.MigrateAsync();
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
    var autorizacionDbContext = scope.ServiceProvider.GetRequiredService<AutorizacionDbContext>();
    await autorizacionDbContext.Database.MigrateAsync();
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
app.MapConsultaMedicaEndpoints();
app.MapLaboratorioExternoEndpoints();
app.MapFacturacionEndpoints();
app.MapInventoryEndpoints();
app.MapNotificationEndpoints();
app.MapPromotionEndpoints();
app.MapAutorizacionEndpoints();

app.Run();
