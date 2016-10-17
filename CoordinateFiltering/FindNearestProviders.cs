using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class FindNearestProviders
    {
        // Returns the *square* of the Euclidean distance.
        public static double Distance(Location a, Location b)
        {
            double xs = b.X - a.X;
            double ys = b.Y - a.Y;
            double zs = b.Z - a.Z;
            return xs * xs + ys * ys + zs * zs;
        }

        public static KdTree.KdTree<double, Provider> CartesianProviderKdTreeFromList(IList<Provider> providers)
        {
            KdTree.KdTree<double, Provider> providersTree = new KdTree.KdTree<double, Provider>(3, new KdTree.Math.DoubleMath());
            foreach (var provider in providers)
            {
                providersTree.Add(provider.Location.Cartesian, provider);
            }
            return providersTree;
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

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> Naive(IList<Customer> customers, IList<Provider> providers, int count)
        {
            var nearestProviders = new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            foreach (var customer in customers)
            {
                nearestProviders.Add(customer, Naive(customer, providers, count));
            }

            return nearestProviders;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> NaiveParallel(IList<Customer> customers, IList<Provider> providers, int count)
        {
            var nearestProviders = new ConcurrentDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            Parallel.ForEach(customers, (customer) =>
            {
                nearestProviders.TryAdd(customer, Naive(customer, providers, count));
            });

            return new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>(nearestProviders);
        }

        public static IList<KeyValuePair<double, Provider>> KdTree(Customer customer, KdTree.KdTree<double, Provider> providers, int count)
        {
            var nearestProviders = providers.GetNearestNeighbours(customer.Location.Cartesian, count);
            var nearestProvidersList = new List<KeyValuePair<double, Provider>>();
            foreach (var provider in nearestProviders)
            {
                nearestProvidersList.Add(new KeyValuePair<double, Provider>(Distance(customer.Location, provider.Value.Location), provider.Value));
            }

            return nearestProvidersList;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTree(IList<Customer> customers, KdTree.KdTree<double, Provider> providers, int count)
        {
            var nearestProviders = new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            foreach (var customer in customers)
            {
                nearestProviders.Add(customer, KdTree(customer, providers, count));
            }

            return nearestProviders;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTreeParallel(IList<Customer> customers, KdTree.KdTree<double, Provider> providers, int count)
        {
            var nearestProviders = new ConcurrentDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            Parallel.ForEach(customers, (customer) =>
            {
                nearestProviders.TryAdd(customer, KdTree(customer, providers, count));
            });

            return new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>(nearestProviders);
        }
    }
}
