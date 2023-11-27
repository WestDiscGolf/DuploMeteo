using Domain;
using Newtonsoft.Json;

namespace Application.Dto
{
    public class HourlyDto
    {
        [JsonProperty("time")]
        public IEnumerable<string> Times { get; set; }

        [JsonProperty("temperature_2m")]
        public IEnumerable<double> Temperatures { get; set; }

        public Hourly ToAggregate(List<DateTime> convertedTimes)
        {
            return new Hourly
            {
                Times = convertedTimes,
                Temperatures = Temperatures.ToList()
            };
        }
    }
}