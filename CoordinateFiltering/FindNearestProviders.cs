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

        public static Accord.Collections.KDTree<Provider> CartesianProviderKdTreeFromList(IList<Provider> providers)
        {
            Accord.Collections.KDTree<Provider> providersTree = new Accord.Collections.KDTree<Provider>(3);
            foreach (var provider in providers)
            {
                providersTree.Add(provider.Location.Cartesian, provider);
            }
            return providersTree;
        }

        // Naive, K Nearest Neighbors
        public static IList<KeyValuePair<double, Provider>> KNearestNaive(Customer customer, IList<Provider> providers, int count)
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
                nearestProviders.Add(customer, KNearestNaive(customer, providers, count));
            }

            return nearestProviders;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> NaiveParallel(IList<Customer> customers, IList<Provider> providers, int count)
        {
            var nearestProviders = new ConcurrentDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            Parallel.ForEach(customers, (customer) =>
            {
                nearestProviders.TryAdd(customer, KNearestNaive(customer, providers, count));
            });

            return new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>(nearestProviders);
        }

        // k-d Tree, K Nearest Neighbors, Simple
        public static IList<KeyValuePair<double, Provider>> KdTree(Customer customer, Accord.Collections.KDTree<Provider> providers, int count)
        {
            var nearestProviders = providers.Nearest(customer.Location.Cartesian, count);
            var nearestProvidersList = new List<KeyValuePair<double, Provider>>();
            foreach (var provider in nearestProviders)
            {
                nearestProvidersList.Add(new KeyValuePair<double, Provider>(provider.Distance, provider.Node.Value));
            }

            return nearestProvidersList;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTree(IList<Customer> customers, Accord.Collections.KDTree<Provider> providers, int count)
        {
            var nearestProviders = new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            foreach (var customer in customers)
            {
                nearestProviders.Add(customer, KdTree(customer, providers, count));
            }

            return nearestProviders;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTreeParallel(IList<Customer> customers, Accord.Collections.KDTree<Provider> providers, int count)
        {
            var nearestProviders = new ConcurrentDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            Parallel.ForEach(customers, (customer) =>
            {
                nearestProviders.TryAdd(customer, KdTree(customer, providers, count));
            });

            return new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>(nearestProviders);
        }

        // k-d Tree, All Within Radius, Simple
        public static IList<KeyValuePair<double, Provider>> KdTree(Customer customer, Accord.Collections.KDTree<Provider> providers, double radius)
        {
            var nearestProviders = providers.Nearest(customer.Location.Cartesian, radius);
            var nearestProvidersList = new List<KeyValuePair<double, Provider>>();
            foreach (var provider in nearestProviders)
            {
                nearestProvidersList.Add(new KeyValuePair<double, Provider>(provider.Distance, provider.Node.Value));
            }

            return nearestProvidersList;
        }

        public static IList<KeyValuePair<double, Provider>> KdTree(Customer customer, Accord.Collections.KDTree<Provider> providers, double radius, int maxCount)
        {
            var nearestProviders = providers.Nearest(customer.Location.Cartesian, radius, maxCount);
            var nearestProvidersList = new List<KeyValuePair<double, Provider>>();
            if (nearestProviders.Count == 0) { return nearestProvidersList; }
            foreach (var provider in nearestProviders)
            {
                // Bug in Accord.NET means the foreach throws a null reference exception if collection isn't completely full,
                // so return early in that case.
                if (provider.Node == null) { break; }
                nearestProvidersList.Add(new KeyValuePair<double, Provider>(provider.Distance, provider.Node.Value));
            }

            return nearestProvidersList;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTree(IList<Customer> customers, Accord.Collections.KDTree<Provider> providers, double radius)
        {
            var nearestProviders = new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            foreach (var customer in customers)
            {
                nearestProviders.Add(customer, KdTree(customer, providers, radius));
            }

            return nearestProviders;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTreeParallel(IList<Customer> customers, Accord.Collections.KDTree<Provider> providers, double radius)
        {
            var nearestProviders = new ConcurrentDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            Parallel.ForEach(customers, (customer) =>
            {
                nearestProviders.TryAdd(customer, KdTree(customer, providers, radius));
            });

            return new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>(nearestProviders);
        }

        // k-d Tree, K Nearest Neighbors, Discriminated
        public static IList<KeyValuePair<double, Provider>> KdTree(Customer customer, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount)
        {
            var customerProviders = new List<KeyValuePair<double, Provider>>();
            foreach (var chain in providers.Chains)
            {
                customerProviders.AddRange(KdTree(customer, chain.Value, 1));
            }
            customerProviders = customerProviders.OrderBy(item => item.Key).Take(chainCount).ToList();
            customerProviders.AddRange(KdTree(customer, providers.Retail, retailCount));
            customerProviders.AddRange(KdTree(customer, providers.Other, otherCount));

            return customerProviders.OrderBy(item => item.Key).ToList();
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTree(IList<Customer> customers, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount)
        {
            var nearestProviders = new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            foreach (var customer in customers)
            {
                nearestProviders.Add(customer, KdTree(customer, providers, chainCount, retailCount, otherCount));
            }

            return nearestProviders;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTreeParallel(IList<Customer> customers, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount)
        {
            var nearestProviders = new ConcurrentDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            Parallel.ForEach(customers, (customer) =>
            {
                nearestProviders.TryAdd(customer, KdTree(customer, providers, chainCount, retailCount, otherCount));
            });

            return new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>(nearestProviders);
        }

        // k-d Tree, All Within Radius, Discriminated
        public static IList<KeyValuePair<double, Provider>> KdTree(Customer customer, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount, double radius)
        {
            var customerProviders = new List<KeyValuePair<double, Provider>>();
            foreach (var chain in providers.Chains)
            {
                customerProviders.AddRange(KdTree(customer, chain.Value, radius, 1));
            }
            customerProviders = customerProviders.OrderBy(item => item.Key).Take(chainCount).ToList();
            customerProviders.AddRange(KdTree(customer, providers.Retail, radius, retailCount));
            customerProviders.AddRange(KdTree(customer, providers.Other, radius, otherCount));

            return customerProviders.OrderBy(item => item.Key).ToList();
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTree(IList<Customer> customers, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount, double radius)
        {
            var nearestProviders = new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            foreach (var customer in customers)
            {
                nearestProviders.Add(customer, KdTree(customer, providers, chainCount, retailCount, otherCount, radius));
            }

            return nearestProviders;
        }

        public static SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> KdTreeParallel(IList<Customer> customers, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount, double radius)
        {
            var nearestProviders = new ConcurrentDictionary<Customer, IList<KeyValuePair<double, Provider>>>();
            Parallel.ForEach(customers, (customer) =>
            {
                nearestProviders.TryAdd(customer, KdTree(customer, providers, chainCount, retailCount, otherCount, radius));
            });

            return new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>(nearestProviders);
        }
    }
}
