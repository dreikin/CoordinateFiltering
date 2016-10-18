using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateFiltering
{
    class ProviderSet
    {
        public IDictionary<string, IList<Provider>> Chains { get; }
        public IList<Provider> Retail { get; }
        public IList<Provider> Other { get; }

        public ProviderSet()
        {
            Chains = new Dictionary<string, IList<Provider>>();
            Retail = new List<Provider>();
            Other = new List<Provider>();
        }
    }
}
