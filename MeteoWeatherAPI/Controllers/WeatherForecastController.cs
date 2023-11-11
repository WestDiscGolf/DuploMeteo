using Application.Dto;
using Application.Service;
using Domain;
using MeteoWeatherAPI.CustomAttribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MeteoWeatherAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpPost]
        public async Task<WeatherForecastDto> SaveWeatherForecast([BindRequired] BasicLatLongDto dto)
        {
            var forecast = await _weatherService.SaveWeatherForecastAync(dto.Latitude, dto.Longitude).ConfigureAwait(false);
            return forecast;
        }

        [HttpGet("{latitude}/{longitude}")]
        [MatchesLatLong]
        public async Task<WeatherForecastDto> GetWeatherForecast([BindRequired] string latitude, [BindRequired] string longitude)
        {
            var forecast = await _weatherService.GetWeatherForecastAsync(latitude, longitude).ConfigureAwait(false);
            return forecast;
        }

        [HttpGet("past-forecasts")]
        public async Task<IEnumerable<BasicLatLongDto>> GetPastForecasts()
        {
            var pastForecasts = await _weatherService.GetPastHistoricLatitudesAndLongitudesAsync().ConfigureAwait(false);
            return pastForecasts;
        }

        [HttpDelete("{latitude}/{longitude}")]
        [MatchesLatLong]
        public async Task DeleteWeatherForecast([BindRequired] string latitude, [BindRequired] string longitude)
        {
            await _weatherService.DeleteWeatherForecastAsync(latitude, longitude).ConfigureAwait(false);
            return;
        }
    }
}