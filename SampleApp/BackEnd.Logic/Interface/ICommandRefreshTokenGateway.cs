using SampleApp.BackEnd.Domain;

namespace SampleApp.BackEnd.Logic.Interface;

public interface ICommandRefreshTokenGateway
{
    Task AddRefreshToken(RefreshToken refreshToken, string email);
}