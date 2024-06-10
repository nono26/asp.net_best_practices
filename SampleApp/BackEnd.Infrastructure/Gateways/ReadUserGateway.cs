using BackEnd.Logic.Interface;
using SampleApp.BackEnd.Domain;
using SampleApp.BackEnd.Logic.Interface;

namespace SampleApp.BackEnd.Infrastructure.Gateways;

public class ReadUserGateway : IReadUserGateway
{
    private readonly ILogger<ReadUserGateway> _logger;

    public ReadUserGateway(ILogger<ReadUserGateway> logger)
    {
        _logger = logger;
    }

    public async Task<User> FindByEmailAsync(string Email)
    {
        if (Email == "Arnaud@gmail.com")

            return new User
            {
                Id = 1,
                Name = "Arnaud",
                Email = "Arnaud@gmail.com",
                Roles = new List<Role>
                    {
                        new Role { Id = 1, Name = "Admin" },
                        new Role { Id = 2, Name = "User" }
                    }
            };
        if (Email == "John@gmail.com")
            return new User
            {
                Id = 2,
                Name = "John",
                Email = "John@gmail.com",
                Roles = new List<Role>
                    {
                        new Role { Id = 2, Name = "User" }
                    }
            };
        return null;
    }

    public async Task<User> GetUserAsync(string userName, string password)
    {
        try
        {
            _logger.LogInformation($"Getting user {userName} from database");
            /*
            var user = await _dbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Name == userName && u.Password == password);
            */

            if (userName == "Arnaud")
                return new User
                {
                    Id = 1,
                    Name = "Arnaud",
                    Email = "Arnaud@gmail.com",
                    Roles = new List<Role>
                    {
                        new Role { Id = 1, Name = "Admin" },
                        new Role { Id = 2, Name = "User" }
                    }
                };
            if (userName == "John")
                return new User
                {
                    Id = 2,
                    Name = "John",
                    Email = "John@gmail.com",
                    Roles = new List<Role>
                    {
                        new Role { Id = 2, Name = "User" }
                    }
                };
            return new NullUser();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading user from database");
            return null;
        }
        finally
        {
            _logger.LogInformation("User retrieved from database");
        }
    }
}