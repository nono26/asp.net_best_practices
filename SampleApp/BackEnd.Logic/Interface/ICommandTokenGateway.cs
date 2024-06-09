using SampleApp.BackEnd.Domain;

namespace SampleApp.BackEnd.Logic.Interface;

public interface ICommandTokenGateway
{
    public TokenResource CreateAccessToken(User user);
}