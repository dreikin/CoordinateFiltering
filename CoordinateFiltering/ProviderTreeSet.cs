using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class ProviderTreeSet
    {
        public IDictionary<string, Accord.Collections.KDTree<Provider>> Chains { get; }
        public Accord.Collections.KDTree<Provider> Retail { get; }
        public Accord.Collections.KDTree<Provider> Other { get; }

        public ProviderTreeSet(ProviderSet providers)
        {
            Chains = new Dictionary<string, Accord.Collections.KDTree<Provider>>();
            foreach (var entry in providers.Chains)
            {
                Chains.Add(entry.Key, FindNearestProviders.CartesianProviderKdTreeFromList(entry.Value));
            }
            Retail = FindNearestProviders.CartesianProviderKdTreeFromList(providers.Retail);
            Other = FindNearestProviders.CartesianProviderKdTreeFromList(providers.Other);
        }
    }
}
