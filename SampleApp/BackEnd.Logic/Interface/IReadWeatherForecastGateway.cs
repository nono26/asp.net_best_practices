public interface IReadWeatherForecastGateway
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync(int days);
    Task<IAsyncEnumerable<WeatherForecast>> GetWeatherForecastIAsyncEnumerableAsync(int days);

}