using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationIndexer.Utilities
{
    class LocationProcessConfiguration
    {
        public IEnumerable<string> Countries { get; set; }
        public IEnumerable<string> ExcludedCountries { get; set; }
        public string Environment { get; set; }
    }
}
