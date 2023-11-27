using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IWeatherDomainService
    {
        Task SaveWeatherForecastAsync(WeatherForecast weatherForecast);
        Task<WeatherForecast> GetWeatherForecastAsync(string id);
        Task<IEnumerable<WeatherForecast>> GetPastWeatherForecastsAsync();
        Task<IEnumerable<HistoricLatLong>> GetPreviousLatLongsAsync();
        Task DeleteWeatherForecastAsync(string weatherId);
    }
}
