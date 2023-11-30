using MeteoWeatherAPI.Dto;
using Microsoft.Extensions.Caching.Memory;

namespace MeteoWeatherAPI.Services;

public class WeatherCacheService : IWeatherService
{
    private readonly IWeatherService _weatherService;
    private readonly IMemoryCache _memoryCache;

    public WeatherCacheService(IWeatherService weatherService, IMemoryCache memoryCache)
    {
        _weatherService = weatherService;
        _memoryCache = memoryCache;
    }

    public Task<WeatherForecastDto> SaveWeatherForecastAync(string latitude, string longitude)
    {
        // saving an item doesn't require caching.
        return _weatherService.SaveWeatherForecastAync(latitude, longitude);
    }

    public async Task<WeatherForecastDto?> GetWeatherForecastAsync(string latitude, string longitude)
    {
        // getting the value needs to check the cache and process accordingly
        var cacheKey = new CacheKey(latitude, longitude).GetCacheKey();
        if (_memoryCache.TryGetValue(cacheKey, out var cachedValue)
            && cachedValue is WeatherForecastDto weatherForecast)
        {
            return weatherForecast;
        }
        
        var latestWeatherForecast  = await _weatherService.GetWeatherForecastAsync(latitude, longitude);

        if (latestWeatherForecast is not null)
        {
            _memoryCache.Set(cacheKey, latestWeatherForecast, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
        }

        return latestWeatherForecast;
    }

    public Task<IEnumerable<BasicLatLongDto>> GetPastHistoricLatitudesAndLongitudesAsync()
    {
        // probably doesn't need caching, every time something is saved/deleted this cache will also
        // need manipulation. Worth it? The complexity of cache invalidating if something is in the cache
        // probably isn't worth the pain.
        return _weatherService.GetPastHistoricLatitudesAndLongitudesAsync();
    }

    public Task DeleteWeatherForecastAsync(string latitude, string longitude)
    {
        _weatherService.DeleteWeatherForecastAsync(latitude, longitude);

        // time to invalidate the cache on the deleted item
        var cacheKey = new CacheKey(latitude, longitude).GetCacheKey();
        _memoryCache.Remove(cacheKey);

        return Task.CompletedTask;
    }
}