using SampleApp.BackEnd.Domain;

namespace SampleApp.BackEnd.Logic.Interface;

public interface ICommandRefreshTokenGateway
{
    void AddRefreshToken(RefreshToken refreshToken, string email);
    Task<string> TakeRefreshTokenAsync(string email, string refreshToken);
}