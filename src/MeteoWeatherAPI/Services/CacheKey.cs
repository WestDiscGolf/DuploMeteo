using MeteoWeatherAPI.Dto;

namespace MeteoWeatherAPI.Services;

public class CacheKey
{
    public CacheKey(string latitude, string longitude)
    {
        Latitude = latitude;
        Longitude = longitude;

        if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
        {
            throw new ApplicationException("Latitude, Longitude are required when creating a cache key");
        }
    }

    public string Latitude { get; private set; }

    public string Longitude { get; private set; }

    public string GetCacheKey() => LatLongKey.Key(Latitude, Longitude);
}