using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class PreviousLatLongDto : BasicLatLongDto
    {
        public string LatLongCombined
        {
            get
            {
                return $"{Latitude}{Longitude}";
            }
        }
    }

    public class BasicLatLongDto
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
