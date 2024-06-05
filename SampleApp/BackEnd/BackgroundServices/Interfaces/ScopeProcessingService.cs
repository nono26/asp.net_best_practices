using SampleApp.BackEnd.BackgroundServices.Interfaces;

namespace SampleApp.BackEnd.BackgroundServices;

internal class ScopedProcessingService : IScopedProcessingService
{
    private readonly ILogger<ScopedProcessingService> _logger;

    public ScopedProcessingService(ILogger<ScopedProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scoped Processing Service is working.");
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Scoped Processing Service is doing background work.");
            //var weatherForecast= weatherForecastGateway.GetWeatherForecast().Where(x=>x.TemperatureC>20);
            //do some work
            await Task.Delay(1000, stoppingToken);
        }
    }
}