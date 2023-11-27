using MeteoWeatherAPI.Dto;

namespace MeteoWeatherAPI.Services;

public interface IWeatherCacheService
{
    void SaveForecast(WeatherForecastDto dto);
    WeatherForecastDto GetForecastDto(CacheKey cacheKey);

    void DeleteForecast(CacheKey key);
}