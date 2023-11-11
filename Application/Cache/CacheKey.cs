using Domain.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Cache
{
    public class CacheKey
    {
        public CacheKey(string latitude, string longitude)
        {
            Latitude = latitude;
            Longitude = longitude;

            if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude)) {
                throw new ApplicationException("Latitude, Longitude are required when creating a cache key");
            }
        }

        public string GetCacheKey() => LatLongKey.Key(Latitude, Longitude);
        public string Latitude { get; private set; }
        public string Longitude { get; private set; }


    }
}
