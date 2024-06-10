using MediatR;
using SampleApp.BackEnd.Logic.Interface;

namespace SampleApp.BackEnd.Logic.Queries;

public class RevokeRefreshTokenQuery : IRequest<bool>
{
    public string RefreshToken { get; init; }
    public string Email { get; init; }

    public RevokeRefreshTokenQuery(string refreshToken, string email)
    {
        RefreshToken = refreshToken;
        Email = email;
    }
}

public class RevokeRefreshTokenQueryHandler : IRequestHandler<RevokeRefreshTokenQuery, bool>
{
    private ICommandRefreshTokenGateway _commandRefreshTokenGateway;
    public RevokeRefreshTokenQueryHandler(ICommandRefreshTokenGateway commandRefreshTokenGateway)
    {
        _commandRefreshTokenGateway = commandRefreshTokenGateway;
    }
    public async Task<bool> Handle(RevokeRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var task = await _commandRefreshTokenGateway.TakeRefreshTokenAsync(request.Email, request.RefreshToken);
        return task != null;
    }
}