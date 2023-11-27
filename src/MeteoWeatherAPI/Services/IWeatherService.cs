using MeteoWeatherAPI.Dto;

namespace MeteoWeatherAPI.Services;

public interface IWeatherService
{
    Task<WeatherForecastDto> SaveWeatherForecastAync(string latitude, string longitude);
    Task<WeatherForecastDto> GetWeatherForecastAsync(string latitude, string longitude);
    Task<IEnumerable<BasicLatLongDto>> GetPastHistoricLatitudesAndLongitudesAsync();
    Task DeleteWeatherForecastAsync(string latitude, string longitude);
}