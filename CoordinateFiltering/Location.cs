using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class Location
    {
        // Geographic coordinates
        public double Latitude { get; }
        public double Longitude { get; }
        // Geometric coordinates
        // * Graph type: unit sphere
        // * Coordinate type: Cartesian (x, y, z)
        // * Relation to Geographic coordinates:
        //   * Same center.
        //   * X goes through (0, 0) in (lat, long)
        //   * Y goes through (0, 90)
        //   * Z goes through (90, 0)
        // Note that this assumes a spherical mapping of latitude and longitude.
        // Ellipsoid mappings require more complex conversions.
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public double[] Cartesian => new double[] { X, Y, Z };
        public float[] Geographic => new float[] { (float)Latitude, (float)Longitude };

        public Location(double latitude, double longitude)
        {
            if (latitude >= -90 && latitude <= 90) { Latitude = latitude; } else { throw new ArgumentOutOfRangeException(); }
            if (longitude >= -180 && longitude <= 180) { Longitude = longitude; } else { throw new ArgumentOutOfRangeException(); }
            X = Math.Cos(Latitude) * Math.Cos(Longitude);
            Y = Math.Cos(Latitude) * Math.Sin(Longitude);
            Z = Math.Sin(Latitude);
        }
    }
}
