using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class Program
    {
        static void Main(string[] args)
        {
            Random randomSource = new Random();
            IList<Customer> customers = GenerateCustomers(40000, randomSource);
            IList<Provider> providers = GenerateProviders(922000, randomSource);

            // Add or remove comments in accordance with what you want to test.
            //TestFindNearestNProvidersNaive(customers, providers);
            TestFindNearestNProvidersNaiveParallel(customers, providers);
            TestFindNearestNProvidersKdTree(customers, providers);

            Console.ReadLine();
            return;

        }

        private static IList<Customer> GenerateCustomers(int count, Random randomSource)
        {
            IList<Customer> customers = new List<Customer>(count);
            for (int i = 0; i < count; i++)
            {
                customers.Add(new Customer(i, new Location(randomSource.NextDouble() * 180 - 90, randomSource.NextDouble() * 360 - 180)));
            }

            return customers;
        }

        private static IList<Provider> GenerateProviders(int count, Random randomSource, int geohashPrecision = 9)
        {
            IList<Provider> providers = new List<Provider>();
            for (int i = 0; i < count; i++)
            {
                providers.Add(new Provider(i, new Location(randomSource.NextDouble() * 180 - 90, randomSource.NextDouble() * 360 - 180)));
            }

            return providers;
        }

        private static void TestFindNearestNProvidersNaive(IList<Customer> customers, IList<Provider> providers)
        {
            var output = new StreamWriter(File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TEMP\\CoordinateFiltering\\Naive.txt")));
            output.WriteLine("customer|provider|distance");
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            // Naive method
            timer.Start();
            foreach (var customer in customers)
            {
                var nearestProviders = FindNearestNProviders.Naive(customer, providers, 200);
                foreach (var provider in nearestProviders)
                {
                    output.WriteLine($"{customer.Id}|{provider.Value.Id}|{provider.Key}");
                }
            }
            timer.Stop();
            output.Close();
            Console.WriteLine($"Naive method: {timer.Elapsed}");
        }

        private static void TestFindNearestNProvidersNaiveParallel(IList<Customer> customers, IList<Provider> providers)
        {
            var nearestProviders = new ConcurrentDictionary<int, IList<KeyValuePair<double, Provider>>>();
            var output = new StreamWriter(File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TEMP\\CoordinateFiltering\\NaiveParallel.txt")));
            output.WriteLine("customer|provider|distance");
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            // Naive method, multithreaded
            timer.Start();
            Parallel.ForEach(customers, (customer) =>
            {
                nearestProviders.TryAdd(customer.Id, FindNearestNProviders.Naive(customer, providers, 200));
            });
            var nearestProvidersList = nearestProviders.OrderBy(item => item.Key).ToList();
            foreach (var customer in nearestProvidersList)
            {
                var customerId = customer.Key;
                foreach (var provider in customer.Value)
                {
                    output.WriteLine($"{customerId}|{provider.Value.Id}|{provider.Key}");
                }
            }
            timer.Stop();
            output.Close();
            Console.WriteLine($"Naive method, Parallel.ForEach: {timer.Elapsed}");
        }

        private static void TestFindNearestNProvidersKdTree(IList<Customer> customers, IList<Provider> providers)
        {
            var output = new StreamWriter(File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TEMP\\CoordinateFiltering\\KdTree.txt")));
            output.WriteLine("customer|provider|distance");
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            // Kd-Tree method
            // * Build tree
            timer.Start();
            KdTree.KdTree<double, Provider> providersTree = new KdTree.KdTree<double, Provider>(3, new KdTree.Math.DoubleMath());
            foreach (var provider in providers)
            {
                providersTree.Add(new double[] { provider.Location.X, provider.Location.Y, provider.Location.Z }, provider);
            }
            timer.Stop();
            var kdTimer = timer.Elapsed;
            Console.WriteLine($"Kd-Tree build time: {kdTimer}");

            // * Find providers
            timer.Reset();
            timer.Start();
            foreach (var customer in customers)
            {
                var nearestProviders = providersTree.GetNearestNeighbours(new double[] { customer.Location.X, customer.Location.Y, customer.Location.Z }, 200);
                foreach (var provider in nearestProviders)
                {

                    output.WriteLine($"{customer.Id}|{provider.Value.Id}|{FindNearestNProviders.Distance(customer.Location, provider.Value.Location)}");
                }
            }
            timer.Stop();
            output.Close();
            Console.WriteLine($"Kd-Tree Method: {timer.Elapsed}");
        }
    }
}