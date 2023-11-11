using Application.Dto.Extensions;
using Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class WeatherForecastDto
    {
        public string Id { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("hourly_units")]
        public HourlyUnitsDto HourlyUnits { get; set; }

        [JsonProperty("hourly")]
        public HourlyDto Hourly { get; set; }
        public DateTime TimeLastUpdatedUtc { get; set; }

        public WeatherForecast ToAggregate()
        {
            var convertedTimes = Hourly.Times.ConvertStringTimesUsingTimezone(Timezone);

            return new WeatherForecast
            {
                Id = Id,
                Latitude = Latitude,
                Longitude = Longitude,
                Timezone = Timezone,
                Hourly = Hourly.ToAggregate(convertedTimes),
                HourlyUnits = HourlyUnits.ToAggregate()
            };
        }
    }
}
