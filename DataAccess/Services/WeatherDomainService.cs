using DataAccess.Context;
using Domain;
using Domain.Keys;
using Domain.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class WeatherDomainService : IWeatherDomainService
    {
        private readonly WeatherContext _context;

        public WeatherDomainService(WeatherContext context)
        {
            _context = context;
        }

        public async Task DeleteWeatherForecastAsync(string weatherId)
        {
            var filter = new FilterDefinitionBuilder<WeatherForecast>().Eq(x => x.Id, weatherId);
            await _context.WeatherForecastContext.DeleteOneAsync(filter).ConfigureAwait(false);
        }

        public async Task<IEnumerable<WeatherForecast>> GetPastWeatherForecastsAsync()
        {
            var result = await _context.WeatherForecastContext.Find(Builders<WeatherForecast>.Filter.Empty).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<HistoricLatLong>> GetPreviousLatLongsAsync()
        {            
            var result = await _context.HistoricLatLongs.Find(Builders<HistoricLatLong>.Filter.Empty).ToListAsync();
            return result;
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync(string id)
        {
            var result = await _context.WeatherForecastContext.Find(x => x.Id == id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            return result;
        }

        public async Task SaveWeatherForecastAsync(WeatherForecast weatherForecast)
        {
            await _context.WeatherForecastContext.ReplaceOneAsync(weatherForecastAggregate => weatherForecastAggregate.Id == weatherForecast.Id, weatherForecast, new ReplaceOptions
            {
                IsUpsert = true
            }).ContinueWith(async x =>
            {
                var key = LatLongKey.Key(weatherForecast.Latitude, weatherForecast.Longitude);
                var historicLatLong = new HistoricLatLong { Id = key, Latitude = weatherForecast.Latitude, Longitude = weatherForecast.Longitude };
                await _context.HistoricLatLongs.ReplaceOneAsync(historicAggregate => historicAggregate.Id == key, historicLatLong, new ReplaceOptions
                {
                    IsUpsert = true
                }).ConfigureAwait(false);
            }, TaskContinuationOptions.RunContinuationsAsynchronously).ConfigureAwait(false);
        }
    }
}
