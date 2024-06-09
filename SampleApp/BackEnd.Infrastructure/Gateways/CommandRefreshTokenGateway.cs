using SampleApp.BackEnd.Domain;
using SampleApp.BackEnd.Infrastructure.Models;
using SampleApp.BackEnd.Logic.Interface;

namespace SampleApp.BackEnd.Infrastructure.Gateways;

public class CommandRefreshTokenGateway : ICommandRefreshTokenGateway
{
    private readonly ISet<RefreshTokenWithEmail> _refreshTokens = new HashSet<RefreshTokenWithEmail>();

    public void AddRefreshToken(RefreshToken refreshToken, string email)
    {
        _refreshTokens.Add(new RefreshTokenWithEmail
        {
            Email = email,
            RefreshToken = refreshToken
        });
    }

    public Task<string> TakeRefreshTokenAsync(string email, string refreshToken)
    {
        var token = _refreshTokens.FirstOrDefault(x => x.Email == email && x.RefreshToken.Token == refreshToken);

        if (token == null)
        {
            return Task.FromResult<string>(string.Empty);
        }
        else
        {
            _refreshTokens.Remove(token);
            return Task.FromResult<string>(token.RefreshToken.Token); ;
        }
    }
}
