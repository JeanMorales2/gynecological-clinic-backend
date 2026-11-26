using Ginecologia.Modules.Prospeccion.Application;
using Ginecologia.Modules.Prospeccion.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Prospeccion.Api;

public sealed record CreateProspectRequest(string NombreCompleto, string Telefono, string? TelefonoAlternativo, string? Email, string? Fuente, string? Notas);
public sealed record UpdateProspectRequest(string NombreCompleto, string Telefono, string? TelefonoAlternativo, string? Email, string? Fuente, ProspectStatus Estado, string? Notas);
public sealed record ConvertProspectRequest(DateOnly FechaNacimiento, string? DocumentoIdentidad, string? Alergias, string? AntecedentesPatologicosPersonales);

public static class ProspeccionEndpoints
{
    public static IEndpointRouteBuilder MapProspeccionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var prospectos = endpoints.MapGroup("/api/prospectos").RequireAuthorization();
        prospectos.MapGet("/", ListAsync);
        prospectos.MapGet("/{id:guid}", GetAsync);
        prospectos.MapPost("/", CreateAsync);
        prospectos.MapPut("/{id:guid}", UpdateAsync);
        prospectos.MapPost("/{id:guid}/convertir", ConvertAsync);
        prospectos.MapPost("/{id:guid}/invitar", InviteAsync);
        return endpoints;
    }

    private static Task<PagedProspectResult<ProspectDto>> ListAsync(string? search, ProspectStatus? estado, int page, int pageSize, ListProspectsUseCase useCase, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(search, estado, page, pageSize, cancellationToken);

    private static async Task<IResult> GetAsync(Guid id, GetProspectUseCase useCase, CancellationToken cancellationToken)
    {
        var prospect = await useCase.ExecuteAsync(id, cancellationToken);
        return prospect is null ? Results.NotFound() : Results.Ok(prospect);
    }

    private static async Task<IResult> CreateAsync(CreateProspectRequest request, CreateProspectUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var prospect = await useCase.ExecuteAsync(new CreateProspectCommand(request.NombreCompleto, request.Telefono, request.TelefonoAlternativo, request.Email, request.Fuente, request.Notas), cancellationToken);
            return Results.Created($"/api/prospectos/{prospect.Id}", prospect);
        }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> UpdateAsync(Guid id, UpdateProspectRequest request, UpdateProspectUseCase useCase, CancellationToken cancellationToken)
    {
        if (request.Estado == ProspectStatus.Convertido)
            return Results.BadRequest(new { message = "El estado Convertido solo se asigna al convertir el prospecto en paciente." });

        try
        {
            var prospect = await useCase.ExecuteAsync(id, new UpdateProspectCommand(request.NombreCompleto, request.Telefono, request.TelefonoAlternativo, request.Email, request.Fuente, request.Estado, request.Notas), cancellationToken);
            return Results.Ok(prospect);
        }
        catch (ProspectNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> ConvertAsync(Guid id, ConvertProspectRequest request, ConvertProspectUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var prospect = await useCase.ExecuteAsync(id, new ConvertProspectCommand(request.FechaNacimiento, request.DocumentoIdentidad, request.Alergias, request.AntecedentesPatologicosPersonales), cancellationToken);
            return Results.Ok(prospect);
        }
        catch (ProspectNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> InviteAsync(Guid id, InviteProspectUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var result = await useCase.ExecuteAsync(id, cancellationToken);
            return Results.Ok(result);
        }
        catch (ProspectNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }
}
