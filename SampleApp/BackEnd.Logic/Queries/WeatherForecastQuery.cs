using BackEnd.Logic.Interface;
using MediatR;

namespace BackEnd.Logic.Queries;

public class WeatherForecastQuery : IRequest<IEnumerable<WeatherForecast>>
{
    public int Days { get; set; }
}

public class WeatherForecastQueryHandler : IRequestHandler<WeatherForecastQuery, IEnumerable<WeatherForecast>>
{
    private readonly ILogger<WeatherForecastQueryHandler> _logger;

    public WeatherForecastQueryHandler(ILogger<WeatherForecastQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<WeatherForecast>> Handle(WeatherForecastQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting weather forecast for {Days} days", request.Days);

        var rng = new Random();
        var forecasts = new List<WeatherForecast>();

        var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        ));

        return forecast;
    }

    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
}
