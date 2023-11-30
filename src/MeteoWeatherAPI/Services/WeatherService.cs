using MeteoWeatherAPI.Dto;
using MeteoWeatherAPI.Dto.Extensions;
using Newtonsoft.Json;

namespace MeteoWeatherAPI.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherDataService _weatherDataService;

    public WeatherService(IWeatherDataService weatherDataService)
    {
        _weatherDataService = weatherDataService;
    }

    public async Task DeleteWeatherForecastAsync(string latitude, string longitude)
    {
        var id = LatLongKey.Key(latitude, longitude);
        var toDelete = await _weatherDataService.GetWeatherForecastAsync(id);

        if (toDelete is null)
        {
            return;
        }

        await _weatherDataService.DeleteWeatherForecastAsync(id);
    }

    public async Task<IEnumerable<BasicLatLongDto>> GetPastHistoricLatitudesAndLongitudesAsync()
    {
        var result = await _weatherDataService.GetPreviousLatLongsAsync();
        var dtos = result.Select(x => new BasicLatLongDto
        {
            Latitude = x.Latitude,
            Longitude = x.Longitude
        });

        return dtos;
    }

    public async Task<WeatherForecastDto?> GetWeatherForecastAsync(string latitude, string longitude)
    {
        var key = LatLongKey.Key(latitude, longitude);
        var result = await _weatherDataService.GetWeatherForecastAsync(key);
        return result?.ToDto();
    }

    public async Task<WeatherForecastDto> SaveWeatherForecastAync(string latitude, string longitude)
    {
        using (var client = new HttpClient())
        {
            var result = await client.GetAsync($"" +
                                               $"https://api.open-meteo.com/v1/forecast?latitude={latitude}" +
                                               $"&longitude={longitude}" +
                                               $"&hourly=temperature_2m" +
                                               $"&temperature_unit=fahrenheit" +
                                               $"&timezone=auto");

            var rawJson = await result.Content.ReadAsStringAsync();

            var parsed = JsonConvert.DeserializeObject<WeatherForecastDto>(rawJson);
            if (parsed == null)
                return new WeatherForecastDto();

            parsed.Id = LatLongKey.Key(latitude, longitude);

            //Overriding returned lat/long as they slightly differ from what was passed in when API returns
            //the data and probably does some small adjustments internally to the provided coordinates.
            //This causes the cache key to not be found if we don't set the lat/long of our parsed object to what
            //the user supplied.
            parsed.Longitude = longitude;
            parsed.Latitude = latitude;

            await SaveWeatherForecastAsync(parsed);
            return parsed;
        }
    }

    private async Task SaveWeatherForecastAsync(WeatherForecastDto dto)
    {
        await _weatherDataService.SaveWeatherForecastAsync(dto.ToAggregate());
    }
}