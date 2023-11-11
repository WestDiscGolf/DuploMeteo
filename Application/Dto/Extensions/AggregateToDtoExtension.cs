using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Extensions
{
    public static class AggregateToDtoExtension
    {
        public static WeatherForecastDto ToDto(this WeatherForecast weatherForecast)
        {
            return new WeatherForecastDto()
            {
                TimeLastUpdatedUtc = weatherForecast.TimeLastUpdatedUtc,
                Latitude = weatherForecast.Latitude,
                Longitude = weatherForecast.Longitude,
                Id = weatherForecast.Id,
                Timezone =  weatherForecast.Timezone,
                Hourly = weatherForecast.Hourly.ToDto(),
                HourlyUnits = weatherForecast.HourlyUnits.ToDto()
            };
        }

        public static HourlyDto ToDto(this Hourly hourly)
        {
            return new HourlyDto()
            {
                Temperatures = hourly.Temperatures,
                Times = hourly.Times.Select(x => x.ToString("O"))
            };
        }

        public static HourlyUnitsDto ToDto(this HourlyUnits hourlyUnits)
        {
            return new HourlyUnitsDto()
            {
                Temperature =  hourlyUnits.Temperature,
                Time = hourlyUnits.Time
            };
        }
    }
}
