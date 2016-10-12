using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class Location
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Location(double latitude, double longitude)
        {
            if (latitude >= -90 && latitude <= 90) { Latitude = latitude; } else { throw new ArgumentOutOfRangeException(); }
            if (longitude >= -180 && longitude <= 180) { Longitude = longitude; } else { throw new ArgumentOutOfRangeException(); }
        }
    }
}
