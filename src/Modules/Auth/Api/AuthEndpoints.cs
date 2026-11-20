using System.IdentityModel.Tokens.Jwt;
using Ginecologia.Modules.Auth.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Auth.Api;

public sealed record LoginRequest(string Email, string Password);

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/api/auth");
        auth.MapPost("/login", LoginAsync);
        auth.MapGet("/me", GetCurrentUserAsync).RequireAuthorization();
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
}
