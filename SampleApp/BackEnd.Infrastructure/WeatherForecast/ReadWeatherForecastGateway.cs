using BackEnd.Logic.Interface;

/// <summary>
/// Read Weather Forecast Gateway, it might read the weather forecast from an external source (database / API / files, ...)
/// </summary>
public class ReadWeatherForecastGateway : IReadWeatherForecastGateway
{
    private readonly ILogger<ReadWeatherForecastGateway> _logger;

    public ReadWeatherForecastGateway(ILogger<ReadWeatherForecastGateway> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Read the weather forecast for the next {days} days
    /// </summary>
    /// <param name="days"></param>
    /// <returns></returns>
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecast(int days)
    {
        _logger.LogInformation($"Getting weather forecast for {days} days");

        var rng = new Random();
        var forecasts = new List<WeatherForecast>();
        //Let's pretend to read from an external source
        var forecast = Enumerable.Range(1, days).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        ));

        return await Task.FromResult(forecast);
    }

    public async Task<IAsyncEnumerable<WeatherForecast>> GetWeatherForecastIAsyncEnumerable(int days)
    {
        _logger.LogInformation($"Getting weather forecast for {days} days");

        var rng = new Random();
        var forecasts = new List<WeatherForecast>();
        //Let's pretend to read from an external source
        var forecast = Enumerable.Range(1, days).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        ));

        return await Task.FromResult(forecast.ToAsyncEnumerable());
    }

    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
}