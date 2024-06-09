using BackEnd.Infrastructure.Logger;
using BackEnd.Logic.Interface;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.BackEnd.Infrastructure.Gateways;
using SampleApp.BackEnd.Logic.Interface;

namespace BackEnd.Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ILogger<>), typeof(ConsoleLogger<>));//Register the ConsoleLogger as ILogger as a singleton because it's a simple implementation of a logger
        services.AddScoped<IReadWeatherForecastGateway, ReadWeatherForecastGateway>(); //Register the ReadWeatherForecastGateway as IReadWeatherForecastGateway as a scoped service because it might have a state
        services.AddScoped<IReadUserGateway, ReadUserGateway>(); //Register the ReadUserGateway as IReadUserGateway
        services.AddSingleton<ICommandRefreshTokenGateway, CommandRefreshTokenGateway>(); //Register the CommandRefreshTokenGateway as ICommandRefreshTokenGateway I need a persistence to fake a database
        services.AddScoped<ICommandTokenGateway, CommandTokenGateway>();
        return services;
    }
}
