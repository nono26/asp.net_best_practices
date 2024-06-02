public interface IReadWeatherForecastGateway
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecast(int days);
    Task<IAsyncEnumerable<WeatherForecast>> GetWeatherForecastIAsyncEnumerable(int days);

}