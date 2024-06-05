
using SampleApp.BackEnd.BackgroundServices.Interfaces;

namespace SampleApp.BackEnd.BackgroundServices;

public class DataConsistencyWorkder : BackgroundService
{
    public IServiceProvider Services { get; }
    public DataConsistencyWorkder(IServiceProvider serviceProvider)
    {
        Services = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        using var scope = Services.CreateScope();
        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            await scopedProcessingService.DoWork(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
        scope.Dispose();
    }
}