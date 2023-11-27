using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class WeatherForecast
    {
        public string Id { get; set; }
        public DateTime TimeLastUpdatedUtc { get; set; } = DateTime.UtcNow;
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Timezone { get; set; }
        public HourlyUnits HourlyUnits { get; set; }
        public Hourly Hourly { get; set; }
    }
}
