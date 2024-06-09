using SampleApp.BackEnd.Domain;

namespace SampleApp.BackEnd.Logic.Interface;
public interface IReadUserGateway
{
    Task<User> GetUser(string UserName, string Password);
}
