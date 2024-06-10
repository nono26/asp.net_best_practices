using MediatR;
using SampleApp.BackEnd.Domain;
using SampleApp.BackEnd.Logic.Interface;

namespace SampleApp.BackEnd.Logic.Queries;

public class RefreshLoginQuery : IRequest<TokenResource>
{
    public string Email { get; set; }
    public string RefreshToken { get; set; }
}

public class RefreshLoginQueryHandler : IRequestHandler<RefreshLoginQuery, TokenResource>
{
    private readonly ICommandRefreshTokenGateway _commandRefreshTokenGateway;
    private readonly ICommandTokenGateway _commandTokenGateway;
    private readonly IReadUserGateway _readUserGateway;

    public RefreshLoginQueryHandler(ICommandRefreshTokenGateway commandRefreshTokenGateway, ICommandTokenGateway commandTokenGateway, IReadUserGateway readUserGateway)
    {
        _commandRefreshTokenGateway = commandRefreshTokenGateway;
        _commandTokenGateway = commandTokenGateway;
        _readUserGateway = readUserGateway;
    }

    public async Task<TokenResource> Handle(RefreshLoginQuery request, CancellationToken cancellationToken)
    {
        var token = await _commandRefreshTokenGateway.TakeRefreshTokenAsync(request.Email, request.RefreshToken);

        if (string.IsNullOrEmpty(token))
        {
            return new NullTokenResource();
        }

        var user = await _readUserGateway.FindByEmailAsync(request.Email);

        if (user is NullUser)
        {
            return new NullTokenResource();
        }
        return _commandTokenGateway.CreateAccessToken(user);

    }
}