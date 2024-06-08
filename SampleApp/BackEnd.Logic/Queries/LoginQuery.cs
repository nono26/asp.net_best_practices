
using BackEnd.Logic.Interface;
using MediatR;

namespace SampleApp.BackEnd.Logic.Queries;

public class LoginQuery : IRequest<string>
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
{
    private readonly ILogger<LoginQueryHandler> _logger;

    public LoginQueryHandler(ILogger<LoginQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        try
        {
            //define the logic here 
            _logger.LogInformation($"Getting user {request.Username} to login");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading ReadUserGateway");
            return null;
        }
        finally
        {
            _logger.LogInformation("User retrieved");
        }
    }

}