using Newtonsoft.Json;

namespace MeteoWeatherAPI.Dto;

public class HourlyUnitsDto
{
    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("temperature_2m")]
    public string Temperature { get; set; }

    public HourlyUnits ToAggregate()
    {
        return new HourlyUnits
        {
            Temperature = Temperature,
            Time = Time
        };
    }
}