
using MediatR;

namespace SampleApp.BackEnd.Logic.Queries;

public class WeatherForecastQueryIAsyncEnumerable : IRequest<IAsyncEnumerable<WeatherForecast>>
{
    public int Days { get; set; }
}
public class IWeatherForecastQueryIAsyncEnumerableHandler : IRequestHandler<WeatherForecastQueryIAsyncEnumerable, IAsyncEnumerable<WeatherForecast>>
{
    private readonly IReadWeatherForecastGateway _gateway;

    public async Task<IAsyncEnumerable<WeatherForecast>> Handle(WeatherForecastQueryIAsyncEnumerable request, CancellationToken cancellationToken)
    {
        return await _gateway.GetWeatherForecastIAsyncEnumerable(request.Days);//Access to the gateway to get the data from an external source
    }
}
