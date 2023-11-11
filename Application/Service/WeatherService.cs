using Application.Cache;
using Application.Dto;
using Application.Dto.Extensions;
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
        Task<WeatherForecastDto> SaveWeatherForecastAync(string latitude, string longitude);
        Task<WeatherForecastDto> GetWeatherForecastAsync(string latitude, string longitude);
        Task<IEnumerable<BasicLatLongDto>> GetPastHistoricLatitudesAndLongitudesAsync();
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
            
            if(toDelete == null)
                return;

            var cacheKey = new CacheKey(toDelete.Latitude, toDelete.Longitude);

            await weatherDomainService.DeleteWeatherForecastAsync(id).ContinueWith(x =>
                {
                    weatherCacheService.DeleteForecast(cacheKey);
                }, TaskContinuationOptions.ExecuteSynchronously)
            .ConfigureAwait(false);
        }

        public async Task<IEnumerable<BasicLatLongDto>> GetPastHistoricLatitudesAndLongitudesAsync()
        {
            var result = await weatherDomainService.GetPreviousLatLongsAsync().ConfigureAwait(false);
            var dtos = result.Select(x => new BasicLatLongDto
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude
            });


            return dtos;
        }

        public async Task<WeatherForecastDto> GetWeatherForecastAsync(string latitude, string longitude)
        {
            WeatherForecastDto cached = GetCachedForecast(latitude, longitude);

            if (cached != null)
            {
                return cached;
            }

            var key = LatLongKey.Key(latitude, longitude);
            var result = await weatherDomainService.GetWeatherForecastAsync(key).ConfigureAwait(false);
            
            if(result == null)
            {
                return null;
            }

            return result.ToDto();
        }

        public async Task<WeatherForecastDto> SaveWeatherForecastAync(string latitude, string longitude)
        {
            WeatherForecastDto cached = GetCachedForecast(latitude, longitude);

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

                await SaveWeatherForecastAsync(parsed).ConfigureAwait(false);
                return parsed;
            }
        }

        private async Task SaveWeatherForecastAsync(WeatherForecastDto dto)
        {
            await weatherDomainService.SaveWeatherForecastAsync(dto.ToAggregate()).ConfigureAwait(false);
            weatherCacheService.SaveForecast(dto);

        }

        private WeatherForecastDto GetCachedForecast(string latitude, string longitude)
        {
            var cacheKey = new CacheKey(latitude, longitude);
            var cached = weatherCacheService.GetForecastDto(cacheKey);
            return cached;
        }
    }
}
