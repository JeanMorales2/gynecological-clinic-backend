using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Auth.Domain;
using Microsoft.IdentityModel.Tokens;

namespace Ginecologia.Modules.Auth.Infrastructure;

public sealed record JwtOptions(string SigningKey)
{
    public const int ExpirationHours = 8;
    public SymmetricSecurityKey CreateSecurityKey() => new(Encoding.UTF8.GetBytes(SigningKey));
}

public sealed class JwtTokenIssuer(JwtOptions options) : ITokenIssuer
{
    public IssuedToken Issue(AuthAccount account)
    {
        var expiresAtUtc = DateTime.UtcNow.AddHours(JwtOptions.ExpirationHours);
        var credentials = new SigningCredentials(options.CreateSecurityKey(), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim("role", account.Role),
            ],
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new IssuedToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
