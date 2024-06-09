
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
using System.Security.AccessControl;

namespace SampleApp.BackEnd.Logic.Queries;

public class LoginQuery : IRequest<TokenResource>
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginQueryHandler : IRequestHandler<LoginQuery, TokenResource>
{
    private readonly ILogger<LoginQueryHandler> _logger;
    private readonly IReadUserGateway _readUserGateway;
    private readonly ICommandRefreshTokenGateway _commandRefreshTokenGateway;
    private readonly ICommandTokenGateway _commandTokenGateway;

    public LoginQueryHandler(ILogger<LoginQueryHandler> logger, IReadUserGateway readUserGateway, ICommandTokenGateway commandTokenGateway, ICommandRefreshTokenGateway commandRefreshTokenGateway)
    {
        _logger = logger;
        _readUserGateway = readUserGateway;
        _commandTokenGateway = commandTokenGateway;
        _commandRefreshTokenGateway = commandRefreshTokenGateway;
    }

    /// <summary>
    /// Handle the login query
    /// </summary>
    public async Task<TokenResource> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        try
        {
            //define the logic here 
            _logger.LogInformation($"Getting user {request.Username} to login");
            //Check if the user exists
            var user = await _readUserGateway.GetUserAsync(request.Username, request.Password);
            if (user is NullUser)
            {
                _logger.LogInformation($"User {request.Username} not found");
                return new NullTokenResource();
            }
            var accessToken = _commandTokenGateway.CreateAccessToken(user);
            _commandRefreshTokenGateway.AddRefreshToken(accessToken.RefreshToken, user.Email);
            return accessToken;
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
}