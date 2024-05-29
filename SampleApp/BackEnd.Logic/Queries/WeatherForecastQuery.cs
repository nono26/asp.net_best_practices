using BackEnd.Logic.Interface;
using MediatR;

namespace BackEnd.Logic.Queries;

/// <summary>
/// Weather Forecast Query (MediatR implementation), kind of DTO, it defines a "query" that will be handled by the WeatherForecastQueryHandler
/// </summary>
public class WeatherForecastQuery : IRequest<IEnumerable<WeatherForecast>>
{
    public int Days { get; set; }
}

/// <summary>
/// Weather Forecast Query Handler (MediatR implementation), it handles the WeatherForecastQuery
/// </summary>
public class WeatherForecastQueryHandler : IRequestHandler<WeatherForecastQuery, IEnumerable<WeatherForecast>>
{
    private readonly ILogger<WeatherForecastQueryHandler> _logger;
    private readonly IReadWeatherForecastGateway _gateway;

    public WeatherForecastQueryHandler(ILogger<WeatherForecastQueryHandler> logger, IReadWeatherForecastGateway gateway)
    {
        _logger = logger;
        _gateway = gateway;
    }

    public async Task<IEnumerable<WeatherForecast>> Handle(WeatherForecastQuery request, CancellationToken cancellationToken)
    {
        try
        {
            //define the logic here 
            _logger.LogInformation($"Getting weather forecast for {request.Days} days");
            return await Task.FromResult(
                await _gateway.GetWeatherForecast(request.Days) //Access to the gateway to get the data from an external source
                );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading ReadWeatherForecastGateway");
            return [];//return an empty list is a acceptable data and when an error occurs to avoid null reference exceptions 
        }
        finally
        {
            _logger.LogInformation("Weather forecast retrieved");
        }
    }
}
