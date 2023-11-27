using MeteoWeatherAPI.Dto;

namespace MeteoWeatherAPI.Services;

public interface IWeatherDomainService
{
    Task SaveWeatherForecastAsync(WeatherForecast weatherForecast);
    Task<WeatherForecast> GetWeatherForecastAsync(string id);
    Task<IEnumerable<WeatherForecast>> GetPastWeatherForecastsAsync();
    Task<IEnumerable<HistoricLatLong>> GetPreviousLatLongsAsync();
    Task DeleteWeatherForecastAsync(string weatherId);
}