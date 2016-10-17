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
        }

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

        public static TestData FindNearestNProvidersKdTree(IList<Customer> customers, KdTree.KdTree<double, Provider> providers, int count)
        {
            // Kd-Tree method
            // * Build tree
            //timer.Start();
            //KdTree.KdTree<double, Provider> providersTree = BuildProviderTree(providers);
            //timer.Stop();
            //var kdTimer = timer.Elapsed;
            //Console.WriteLine($"Kd-Tree build time: {kdTimer}");

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTree(customers, providers, count);
            timer.Stop();
            
            return new TestData(timer.Elapsed, nearestProviders);
        }

        public static TestData FindNearestNProvidersKdTreeParallel(IList<Customer> customers, KdTree.KdTree<double, Provider> providers, int count)
        {
            // Kd-Tree method
            // * Build tree
            //timer.Start();
            //KdTree.KdTree<double, Provider> providersTree = BuildProviderTree(providers);
            //timer.Stop();
            //var kdTimer = timer.Elapsed;
            //Console.WriteLine($"Kd-Tree build time: {kdTimer}");
            
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var nearestProviders = FindNearestProviders.KdTreeParallel(customers, providers, count);
            timer.Stop();
            
            return new TestData(timer.Elapsed, nearestProviders);
        }
    }
}
