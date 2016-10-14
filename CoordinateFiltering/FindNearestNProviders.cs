using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class FindNearestNProviders
    {
        // Returns the *square* of the Euclidean distance.
        public static double Distance(Location a, Location b)
        {
            double xs = b.X - a.X;
            double ys = b.Y - a.Y;
            double zs = b.Z - a.Z;
            return xs * xs + ys * ys + zs * zs;
        }
        public static IList<KeyValuePair<double, Provider>> Naive(Customer customer, IList<Provider> providers, int count)
        {
            IDictionary<double, Provider> sortedProviders = new SortedDictionary<double, Provider>();
            double maxDistance = 0;
            foreach(var provider in providers)
            {
                double distance = Distance(customer.Location, provider.Location);
                if(sortedProviders.Count < count)
                {
                    maxDistance = distance > maxDistance ? distance : maxDistance;
                    sortedProviders.Add(distance, provider);
                }
                else if (distance < maxDistance)
                {
                    sortedProviders.Add(distance, provider);
                    maxDistance = sortedProviders.Last().Key;
                    sortedProviders.Remove(sortedProviders.Last());
                }
            }

            return sortedProviders.ToList();
        }
    }
}
