using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class Provider
    {
        public int Id { get; }
        public Location Location { get; }

        public Provider(int id, Location location)
        {
            Id = id;
            Location = location;
        }
    }
}
