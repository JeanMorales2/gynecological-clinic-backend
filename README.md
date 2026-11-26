# Ginecología — Backend

API del software de gestión para una clínica de ginecología. Monolito modular en .NET con arquitectura hexagonal por módulo (Domain / Application / Infrastructure / Api), EF Core contra PostgreSQL, un `DbContext` por módulo con su propio esquema.

Este repositorio es la **fuente de despliegue** del servicio backend en Railway (build vía `Dockerfile`). El desarrollo día a día ocurre en el monorepo original; este repo se sincroniza desde ahí.

## Stack

- .NET 9 (`net9.0`)
- ASP.NET Core Minimal APIs
- Entity Framework Core + Npgsql (PostgreSQL)
- Autenticación JWT propia (hash de contraseña, sin login externo)
- Resend (API HTTP de envío de correo, módulo Notificaciones)
- Swagger/OpenAPI (Swashbuckle), solo habilitado en `Development`

## Módulos

Auth · Usuarios y roles · Pacientes · Prospección · Citas/Agenda · Historia clínica · Laboratorio externo · Facturación · Inventario/stock · Promociones · Notificaciones.

Cada módulo vive bajo `src/Modules/<Modulo>/` con sus capas `Domain/`, `Application/`, `Infrastructure/` y `Api/`. `src/Host/` ensambla todos los módulos (`Program.cs`) y expone la app ASP.NET Core.

## Variables de entorno

| Variable | Descripción |
|---|---|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión Npgsql a PostgreSQL |
| `JWT_SIGNING_KEY` | Clave de firma de los JWT emitidos por Auth |
| `AUTH_SEED_ADMIN_PASSWORD` | Contraseña del usuario Admin sembrado en el primer arranque |
| `RESEND_API_KEY` | API key de Resend para el módulo Notificaciones |
| `RESEND_FROM_EMAIL` / `RESEND_FROM_NAME` | Remitente de los correos enviados (el dominio de `RESEND_FROM_EMAIL` debe estar verificado en Resend) |

Las migraciones de EF Core de cada módulo se aplican automáticamente al arrancar (`Database.MigrateAsync()`), no requieren un paso manual en el deploy.

## Desarrollo local

```bash
cd src/Host
ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only" \
JWT_SIGNING_KEY="development-only-signing-key" \
AUTH_SEED_ADMIN_PASSWORD="CambiameYa123!" \
dotnet run
```

Requiere una instancia de PostgreSQL accesible (ver el monorepo `Ginecologia` para `docker-compose.yml` con Postgres incluido). En `Development`, Swagger UI queda disponible en `/swagger`.

## Build

```bash
dotnet build Backend.sln
```

## Docker

```bash
docker build -t ginecologia-backend .
docker run -p 8080:8080 --env-file .env ginecologia-backend
```
