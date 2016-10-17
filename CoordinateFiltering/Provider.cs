using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class Provider
    {
        public int Id { get; }
        public Location Location { get; }
        public string Category { get; }

        public string DedupeKey { get; }

        public Provider(int id, Location location)
            : this(id, location, "", "")
        {}

        public Provider(int id, Location location, string category, string dedupeKey)
        {
            Id = id;
            Location = location;
            Category = category;
            DedupeKey = dedupeKey;
        }
    }
}
