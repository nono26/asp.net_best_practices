public interface IReadWeatherForecastGateway
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecast(int days);
}