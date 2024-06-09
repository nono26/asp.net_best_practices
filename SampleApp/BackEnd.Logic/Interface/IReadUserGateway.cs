using SampleApp.BackEnd.Domain;

namespace SampleApp.BackEnd.Logic.Interface;
public interface IReadUserGateway
{
    Task<User> GetUserAsync(string UserName, string Password);

    Task<User> FindByEmailAsync(string Email);
}
