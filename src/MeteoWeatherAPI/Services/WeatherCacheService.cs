using Application.Dto;
using Microsoft.Extensions.Caching.Memory;

namespace MeteoWeatherAPI.Services;

public interface IWeatherCacheService
{
    void SaveForecast(WeatherForecastDto dto);
    WeatherForecastDto GetForecastDto(CacheKey cacheKey);

    void DeleteForecast(CacheKey key);
}

public class WeatherCacheService : IWeatherCacheService
{
    private readonly IMemoryCache _memoryCache;

    public WeatherCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public void DeleteForecast(CacheKey key)
    {
        var cacheKey = key.GetCacheKey();

        if (_memoryCache.TryGetValue(cacheKey, out _))
            _memoryCache.Remove(key.GetCacheKey());
    }

    public WeatherForecastDto GetForecastDto(CacheKey cacheKey)
    {
        var key = cacheKey.GetCacheKey();

        var result = _memoryCache.Get<WeatherForecastDto>(key);
        return result;
    }

    public void SaveForecast(WeatherForecastDto dto)
    {
        var key = new CacheKey(dto.Latitude, dto.Longitude).GetCacheKey();
        _memoryCache.Set(key, dto, new MemoryCacheEntryOptions()
        {
            //Reasonable cache for weather data
            SlidingExpiration = TimeSpan.FromMinutes(30)
        });

    }
}