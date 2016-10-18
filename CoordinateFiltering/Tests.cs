using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class Tests
    {
        public class TestData
        {
            public TimeSpan Elapsed { get; }
            public SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> NearestProviders { get; }
            public TestData(TimeSpan elapsed, SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> nearestProviders)
            {
                Elapsed = elapsed;
                NearestProviders = nearestProviders;
            }

            public TestData(TimeSpan elapsed, IDictionary<Customer, IList<KeyValuePair<double, Provider>>> nearestProviders)
            {
                Elapsed = elapsed;
                NearestProviders = new SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>>(nearestProviders);
            }
        }

        // Naive, K Nearest Neighbors
        public static TestData FindNearestNProvidersNaive(IList<Customer> customers, IList<Provider> providers, int count)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.Naive(customers, providers, count);
            timer.Stop();

            return new TestData(timer.Elapsed, nearestProviders);
        }

        public static TestData FindNearestNProvidersNaiveParallel(IList<Customer> customers, IList<Provider> providers, int count)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.NaiveParallel(customers, providers, count);
            timer.Stop();
            
            return new TestData(timer.Elapsed, nearestProviders);
        }

        // k-d Tree, K Nearest Neighbors, Simple
        public static TestData FindNearestNProvidersKdTree(IList<Customer> customers, Accord.Collections.KDTree<Provider> providers, int count)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTree(customers, providers, count);
            timer.Stop();
            
            return new TestData(timer.Elapsed, nearestProviders);
        }

        public static TestData FindNearestNProvidersKdTreeParallel(IList<Customer> customers, Accord.Collections.KDTree<Provider> providers, int count)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTreeParallel(customers, providers, count);
            timer.Stop();
            
            return new TestData(timer.Elapsed, nearestProviders);
        }

        // k-d Tree, All Within Radius, Simple
        internal static TestData FindProvidersWithinRadiusKdTree(IList<Customer> customers, Accord.Collections.KDTree<Provider> providers, double radius)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTree(customers, providers, radius);
            timer.Stop();

            return new TestData(timer.Elapsed, nearestProviders);
        }

        internal static TestData FindProvidersWithinRadiusKdTreeParallel(IList<Customer> customers, Accord.Collections.KDTree<Provider> providers, double radius)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTreeParallel(customers, providers, radius);
            timer.Stop();

            return new TestData(timer.Elapsed, nearestProviders);
        }

        // k-d Tree, K Nearest Neighbors, Discriminated
        public static TestData FindNearestNProvidersKdTree(IList<Customer> customers, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTree(customers, providers, chainCount, retailCount, otherCount);
            timer.Stop();

            return new TestData(timer.Elapsed, nearestProviders);
        }

        public static TestData FindNearestNProvidersKdTreeParallel(IList<Customer> customers, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTreeParallel(customers, providers, chainCount, retailCount, otherCount);
            timer.Stop();

            return new TestData(timer.Elapsed, nearestProviders);
        }

        // k-d Tree, All Within Radius, Discriminated
        internal static TestData FindProvidersWithinRadiusKdTree(IList<Customer> customers, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount, double radius)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTree(customers, providers, chainCount, retailCount, otherCount, radius);
            timer.Stop();

            return new TestData(timer.Elapsed, nearestProviders);
        }

        internal static TestData FindProvidersWithinRadiusKdTreeParallel(IList<Customer> customers, ProviderTreeSet providers, int chainCount, int retailCount, int otherCount, double radius)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTreeParallel(customers, providers, chainCount, retailCount, otherCount, radius);
            timer.Stop();

            return new TestData(timer.Elapsed, nearestProviders);
        }
    }
}
