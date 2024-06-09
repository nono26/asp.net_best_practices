using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Logic;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleApp.BackEnd.Domain;
using SampleApp.BackEnd.Logic.Interface;

namespace SampleApp.BackEnd.Infrastructure.Gateways;

public class CommandTokenGateway : ICommandTokenGateway
{
    private readonly TokenOptions _tokenOptions;

    public CommandTokenGateway(IOptions<TokenOptions> tokenOptions)
    {
        _tokenOptions = tokenOptions.Value;
    }

    public TokenResource CreateAccessToken(User user)
    {
        var refreshToken = BuildRefreshToken();
        var accessToken = BuildAccessToken(user, refreshToken);
        return accessToken;
    }

    private TokenResource BuildAccessToken(User user, RefreshToken refreshToken)
    {
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);

        var securityToken = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: GetClaims(user),
            expires: accessTokenExpiration,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Secret)), SecurityAlgorithms.HmacSha256)
        );

        var handler = new JwtSecurityTokenHandler();
        return new TokenResource(handler.WriteToken(securityToken), accessTokenExpiration.Ticks, refreshToken);
    }

    private IEnumerable<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        return claims;
    }

    private RefreshToken BuildRefreshToken()
    {
        var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.RefreshTokenExpiration);
        return new RefreshToken(Guid.NewGuid().ToString(), refreshTokenExpiration.Ticks);
    }
}