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
            var workingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TEMP\\CoordinateFiltering");


            RunRandomDataTests(workingDirectory);

            if (args.Length == 1)
            {
                RunProviderCsvDataTests(args[0], workingDirectory);
            }

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

        private static void RunRandomDataTests(string workingDirectory)
        {
            // Set up output paths.
            var outputPaths = new Dictionary<string, string>();
            outputPaths.Add("naivePath", Path.Combine(workingDirectory, "RandomData\\Naive.txt"));
            outputPaths.Add("naiveParallelPath", Path.Combine(workingDirectory, "RandomData\\NaiveParallel.txt"));
            outputPaths.Add("kdTreePath", Path.Combine(workingDirectory, "RandomData\\KdTree.txt"));
            outputPaths.Add("kdTreeParallelPath", Path.Combine(workingDirectory, "RandomData\\KdTreeParallel.txt"));

            // Generate data for tests.
            Random randomSource = new Random();
            IList<Customer> customers = GenerateCustomers(40000, randomSource);
            IList<Provider> providers = GenerateProviders(922000, randomSource);

            // Run tests.
            RunDataTests(customers, providers, outputPaths, "Randomly generated");
            Console.ReadLine();
        }

        private static void RunProviderCsvDataTests(string inputFile, string workingDirectory)
        {
            var outputPaths = new Dictionary<string, string>();
            outputPaths.Add("naivePath", Path.Combine(workingDirectory, "CsvData\\Naive.txt"));
            outputPaths.Add("naiveParallelPath", Path.Combine(workingDirectory, "CsvData\\NaiveParallel.txt"));
            outputPaths.Add("kdTreePath", Path.Combine(workingDirectory, "CsvData\\KdTree.txt"));
            outputPaths.Add("kdTreeParallelPath", Path.Combine(workingDirectory, "CsvData\\KdTreeParallel.txt"));

            // Generate data for tests.
            Random randomSource = new Random();
            IList<Customer> customers = GenerateCustomers(40000, randomSource);

            /*
             * Provider CSV format expected is:
             *   ID|Latitude|Longitude|Category|DedupeKey
             * with types:
             *   int|double|double|string|hex-string
             *   
             * DedupeKey is optional, everything else is required.
             */
            IList<Provider> providers = new List<Provider>();
            var input = Path.Combine(workingDirectory, inputFile);
            {
                foreach (string line in File.ReadLines(input))
                {
                    var fields = line.Split('|');
                    var id = Int32.Parse(fields[0]);
                    var location = new Location(Double.Parse(fields[1]), Double.Parse(fields[2]));
                    var category = fields[3];
                    var dedupeKey = fields[4];
                    providers.Add(new Provider(id, location, category, dedupeKey));
                }
            }

            // Run tests.
            RunDataTests(customers, providers, outputPaths, "CSV of providers, randomly generated customers");
            Console.ReadLine();
        }

        private static void RunDataTests(IList<Customer> customers, IList<Provider> providers, IDictionary<string, string> outputPaths, string dataType)
        {
            KdTree.KdTree<double, Provider> providersTree = FindNearestProviders.CartesianProviderKdTreeFromList(providers);
            int count = 200;
            Tests.TestData result;

            // Comment blocks in accordance with what you want to test.
            Console.WriteLine($"Data type: {dataType}");

            /*
             * Naive algorithm tests.
             */
            //result = Tests.FindNearestNProvidersNaive(customers, providers, count);
            //Console.WriteLine($"Naive method: {result.Elapsed}");
            //PrintNearestProviders(result.NearestProviders, outputPaths["naivePath"]);

            //result = Tests.FindNearestNProvidersNaiveParallel(customers, providers, count);
            //Console.WriteLine($"Naive method, Parallel.ForEach: {result.Elapsed}");
            //PrintNearestProviders(result.NearestProviders, outputPaths["naiveParallelPath"]);

            /*
             * k-d tree tests.
             */
            result = Tests.FindNearestNProvidersKdTree(customers, providersTree, count);
            Console.WriteLine($"Kd-Tree Method: {result.Elapsed}");
            PrintNearestProviders(result.NearestProviders, outputPaths["kdTreePath"]);

            result = Tests.FindNearestNProvidersKdTreeParallel(customers, providersTree, count);
            Console.WriteLine($"Kd-Tree, Parallel.ForEach Method: {result.Elapsed}");
            PrintNearestProviders(result.NearestProviders, outputPaths["kdTreeParallelPath"]);
        }

        private static void PrintNearestProviders(SortedDictionary<Customer, IList<KeyValuePair<double, Provider>>> nearestProviders, string outputPath)
        {
            var output = new StreamWriter(File.Open(outputPath, FileMode.Create, FileAccess.Write));
            output.WriteLine("customer|provider|distance");
            foreach (var customer in nearestProviders)
            {
                foreach (var provider in customer.Value)
                {
                    output.WriteLine($"{customer.Key.Id}|{provider.Value.Id}|{provider.Key}");
                }
            }
            output.Close();
        }
    }
}