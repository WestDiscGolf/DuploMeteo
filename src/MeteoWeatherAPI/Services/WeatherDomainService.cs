using Domain.Services;
using MeteoWeatherAPI.Data;
using MeteoWeatherAPI.Dto;
using MongoDB.Driver;

namespace DataAccess.Services;

public class WeatherDomainService : IWeatherDomainService
{
    private readonly WeatherDbContext _dbContext;

    public WeatherDomainService(WeatherDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteWeatherForecastAsync(string weatherId)
    {
        var filter = new FilterDefinitionBuilder<WeatherForecast>().Eq(x => x.Id, weatherId);
        await _dbContext.WeatherForecastContext.DeleteOneAsync(filter).ConfigureAwait(false);
    }

    public async Task<IEnumerable<WeatherForecast>> GetPastWeatherForecastsAsync()
    {
        var result = await _dbContext.WeatherForecastContext.Find(Builders<WeatherForecast>.Filter.Empty).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<HistoricLatLong>> GetPreviousLatLongsAsync()
    {            
        var result = await _dbContext.HistoricLatLongs.Find(Builders<HistoricLatLong>.Filter.Empty).ToListAsync();
        return result;
    }

    public async Task<WeatherForecast> GetWeatherForecastAsync(string id)
    {
        var result = await _dbContext.WeatherForecastContext.Find(x => x.Id == id)
            .SingleOrDefaultAsync()
            .ConfigureAwait(false);

        return result;
    }

    public async Task SaveWeatherForecastAsync(WeatherForecast weatherForecast)
    {
        await _dbContext.WeatherForecastContext.ReplaceOneAsync(weatherForecastAggregate => weatherForecastAggregate.Id == weatherForecast.Id, weatherForecast, new ReplaceOptions
        {
            IsUpsert = true
        }).ContinueWith(async x =>
        {
            var key = LatLongKey.Key(weatherForecast.Latitude, weatherForecast.Longitude);
            var historicLatLong = new HistoricLatLong { Id = key, Latitude = weatherForecast.Latitude, Longitude = weatherForecast.Longitude };
            await _dbContext.HistoricLatLongs.ReplaceOneAsync(historicAggregate => historicAggregate.Id == key, historicLatLong, new ReplaceOptions
            {
                IsUpsert = true
            }).ConfigureAwait(false);
        }, TaskContinuationOptions.RunContinuationsAsynchronously).ConfigureAwait(false);
    }
}