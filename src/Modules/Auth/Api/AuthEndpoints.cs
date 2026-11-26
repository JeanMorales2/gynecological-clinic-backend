using System.IdentityModel.Tokens.Jwt;
using Ginecologia.Modules.Auth.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Auth.Api;

public sealed record LoginRequest(string Email, string Password);
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public sealed record RequestPasswordResetRequest(string Email);
public sealed record ResetPasswordRequest(string Token, string NewPassword);

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/api/auth");
        auth.MapPost("/login", LoginAsync);
        auth.MapGet("/me", GetCurrentUserAsync).RequireAuthorization();
        auth.MapPost("/cambiar-password", ChangePasswordAsync).RequireAuthorization();
        auth.MapPost("/solicitar-reset", RequestPasswordResetAsync);
        auth.MapPost("/resetear-password", ResetPasswordAsync);
        return endpoints;
    }

    private static async Task<IResult> LoginAsync(LoginRequest request, LoginUseCase login, CancellationToken cancellationToken)
    {
        var result = await login.ExecuteAsync(request.Email, request.Password, cancellationToken);
        return result is null ? Results.Unauthorized() : Results.Ok(result);
    }

    private static async Task<IResult> GetCurrentUserAsync(HttpContext context, GetCurrentUserUseCase currentUser, CancellationToken cancellationToken)
    {
        var subject = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(subject, out var accountId)) return Results.Unauthorized();

        var user = await currentUser.ExecuteAsync(accountId, cancellationToken);
        return user is null ? Results.Unauthorized() : Results.Ok(user);
    }

    private static async Task<IResult> ChangePasswordAsync(ChangePasswordRequest request, HttpContext context, ChangePasswordUseCase changePassword, CancellationToken cancellationToken)
    {
        var subject = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(subject, out var accountId)) return Results.Unauthorized();

        try
        {
            await changePassword.ExecuteAsync(accountId, request.CurrentPassword, request.NewPassword, cancellationToken);
            return Results.NoContent();
        }
        catch (KeyNotFoundException) { return Results.Unauthorized(); }
        catch (InvalidCurrentPasswordException exception) { return Results.BadRequest(new { message = exception.Message }); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> RequestPasswordResetAsync(RequestPasswordResetRequest request, RequestPasswordResetUseCase useCase, CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(request.Email, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request, ResetPasswordUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            await useCase.ExecuteAsync(request.Token, request.NewPassword, cancellationToken);
            return Results.NoContent();
        }
        catch (InvalidResetTokenException exception) { return Results.BadRequest(new { message = exception.Message }); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }
}
