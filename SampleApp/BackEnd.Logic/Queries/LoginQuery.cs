
using System.Security.Claims;
using BackEnd.Logic;
using BackEnd.Logic.Interface;
using MediatR;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SampleApp.BackEnd.Domain;
using SampleApp.BackEnd.Logic.Interface;

namespace SampleApp.BackEnd.Logic.Queries;

public class LoginQuery : IRequest<TokenResource>
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginQueryHandler : IRequestHandler<LoginQuery, TokenResource>
{
    private readonly ILogger<LoginQueryHandler> _logger;
    private readonly TokenOptions _tokenOptions;
    private readonly IReadUserGateway _readUserGateway;

    public LoginQueryHandler(ILogger<LoginQueryHandler> logger, IOptions<TokenOptions> tokenOptions, IReadUserGateway readUserGateway)
    {
        _logger = logger;
        _tokenOptions = tokenOptions.Value;
        _readUserGateway = readUserGateway;
    }


    public async Task<TokenResource> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        try
        {
            //define the logic here 
            _logger.LogInformation($"Getting user {request.Username} to login");
            //Check if the user exists
            var user = await _readUserGateway.GetUser(request.Username, request.Password);
            if (user is NullUser)
            {
                _logger.LogInformation($"User {request.Username} not found");
                return new NullTokenResource();
            }
            return BuildAccessToken(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading ReadUserGateway");
            return new NullTokenResource();
        }
        finally
        {
            _logger.LogInformation("User retrieved");
        }
    }

    private TokenResource BuildAccessToken(User user)
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
        return new TokenResource(handler.WriteToken(securityToken), accessTokenExpiration.Ticks);
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

}