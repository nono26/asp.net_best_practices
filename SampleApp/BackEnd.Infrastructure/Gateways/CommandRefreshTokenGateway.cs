using SampleApp.BackEnd.Domain;
using SampleApp.BackEnd.Infrastructure.Models;
using SampleApp.BackEnd.Logic.Interface;

namespace SampleApp.BackEnd.Infrastructure.Gateways;

public class CommandRefreshTokenGateway : ICommandRefreshTokenGateway
{
    private readonly ISet<RefreshTokenWithEmail> _refreshTokens = new HashSet<RefreshTokenWithEmail>();

    public Task AddRefreshToken(RefreshToken refreshToken, string email)
    {
        _refreshTokens.Add(new RefreshTokenWithEmail
        {
            Email = email,
            RefreshToken = refreshToken
        });

        return Task.CompletedTask;
    }
}
