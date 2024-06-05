namespace SampleApp.BackEnd.BackgroundServices.Interfaces;

public interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}
