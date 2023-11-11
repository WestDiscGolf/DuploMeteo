using Application.Cache;
using Application.Dto;
using Domain;
using Domain.Keys;
using Domain.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public interface IWeatherService
    {
        Task SaveWeatherForecastAsync(WeatherForeCastDto dto);
        Task<WeatherForeCastDto> SaveWeatherForecastAync(string latitude, string longitude);
        Task<WeatherForecast> GetWeatherForecastAsync(string latitude, string longitude);
        Task<IEnumerable<PreviousLatLongDto>> GetPastHistoricLatitudesAndLongitudesAsync();
        Task DeleteWeatherForecastAsync(string latitude, string longitude);
    }

    public class WeatherService : IWeatherService
    {
        private readonly IWeatherCacheService weatherCacheService;
        private readonly IWeatherDomainService weatherDomainService;

        public WeatherService(IWeatherCacheService weatherCacheService, IWeatherDomainService weatherDomainService)
        {
            this.weatherCacheService = weatherCacheService;
            this.weatherDomainService = weatherDomainService;
        }
        public async Task DeleteWeatherForecastAsync(string latitude, string longitude)
        {
            var id = LatLongKey.Key(latitude, longitude);
            var toDelete = await weatherDomainService.GetWeatherForecastAsync(id).ConfigureAwait(false);
            var cacheKey = new CacheKey(toDelete.Latitude, toDelete.Longitude);

            await weatherDomainService.DeleteWeatherForecastAsync(id).ContinueWith(x =>
                {
                    weatherCacheService.DeleteForecast(cacheKey);
                }, TaskContinuationOptions.ExecuteSynchronously)
            .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PreviousLatLongDto>> GetPastHistoricLatitudesAndLongitudesAsync()
        {
            var result = await weatherDomainService.GetPreviousLatLongsAsync().ConfigureAwait(false);
            var dtos = result.Select(x => new PreviousLatLongDto
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude
            });


            return dtos;
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync(string latitude, string longitude)
        {
            WeatherForeCastDto cached = GetCachedForecast(latitude, longitude);

            if (cached != null)
            {
                return cached.ToAggregate();
            }

            var key = LatLongKey.Key(latitude, longitude);
            var result = await weatherDomainService.GetWeatherForecastAsync(key).ConfigureAwait(false);
            
            return result;
        }

        public async Task<WeatherForeCastDto> SaveWeatherForecastAync(string latitude, string longitude)
        {
            WeatherForeCastDto cached = GetCachedForecast(latitude, longitude);

            if (cached != null)
            {
                return cached;
            }

            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"" +
                    $"https://api.open-meteo.com/v1/forecast?latitude={latitude}" +
                    $"&longitude={longitude}" +
                    $"&hourly=temperature_2m" +
                    $"&temperature_unit=fahrenheit" +
                    $"&timezone=auto").ConfigureAwait(false);

                var rawJson = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                var parsed = JsonConvert.DeserializeObject<WeatherForeCastDto>(rawJson);
                if (parsed == null)
                    return new WeatherForeCastDto();

                parsed.Id = LatLongKey.Key(latitude, longitude);

                //Overriding returned lat/long as they slightly differ from what was passed in when API returns
                //This causes the cache to always miss on second rerun.
                parsed.Longitude = longitude;
                parsed.Latitude = latitude;

                await SaveWeatherForecastAsync(parsed).ConfigureAwait(false);
                return parsed;
            }
        }

        public async Task SaveWeatherForecastAsync(WeatherForeCastDto dto)
        {
            await weatherDomainService.SaveWeatherForecastAsync(dto.ToAggregate()).ConfigureAwait(false);
            weatherCacheService.SaveForecast(dto);

        }

        private WeatherForeCastDto GetCachedForecast(string latitude, string longitude)
        {
            var cacheKey = new CacheKey(latitude, longitude);
            var cached = weatherCacheService.GetForeCastDto(cacheKey);
            return cached;
        }
    }
}
