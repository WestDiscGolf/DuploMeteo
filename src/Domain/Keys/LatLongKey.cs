using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Keys
{
    public static class LatLongKey
    {
        public static string Key(string latitude, string longitude) => $"Latitude#{latitude}_Longitude#{longitude}";
    }
}
