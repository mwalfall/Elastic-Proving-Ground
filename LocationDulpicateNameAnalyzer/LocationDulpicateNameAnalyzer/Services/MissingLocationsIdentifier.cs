using LocationDulpicateNameAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationDulpicateNameAnalyzer.Services
{
    public class MissingLocationsIdentifier
    {
        private AdoClientService _adoClientService;
        private ElasticClientService _esClient;

        public MissingLocationsIdentifier(AdoClientService adoClientService, ElasticClientService esClient)
        {
            _adoClientService = adoClientService;
            _esClient = esClient;

        }

        public void Process()
        {
            List<Tuple<long, string, string>> missingLocations = new List<Tuple<long, string, string>>();
            var cnt = 0;
            var countryCodes = _adoClientService.GetCountryCodes();

            var client = _esClient.GetClient();

            foreach(var countryCode in countryCodes)
            {
                Console.WriteLine("Processing: {0}", countryCode);

                var ids = _adoClientService.GetLocationIds(countryCode);
                Console.WriteLine("---> {0} locations to process.", ids.Count());
                Console.WriteLine(" ");

                foreach( var id in ids)
                {
                    var locationForLargerIndex = client.Get<ElasticsearchLocation>(g => g.Index("locations-en-08.29.16.13.25.30").Type("location").Id(id));
                    if (locationForLargerIndex != null && locationForLargerIndex.Source != null)
                    {
                        var locationForSmallerIndex = client.Get<ElasticsearchLocation>(g => g.Index("locations-en-08.29.16.11.52.46").Type("location").Id(id));
                        if (locationForSmallerIndex == null || locationForSmallerIndex.Source == null || locationForSmallerIndex.Source.ID != locationForLargerIndex.Source.ID)
                        {
                            cnt++;
                            Console.WriteLine(string.Format("{0} : {1}, {2} -- {3}", cnt, locationForLargerIndex.Source.ID, locationForLargerIndex.Source.FormattedName, locationForLargerIndex.Source.CountryCode));
                            missingLocations.Add(new Tuple<long, string, string>(locationForLargerIndex.Source.ID, locationForLargerIndex.Source.FormattedName, locationForLargerIndex.Source.CountryCode));
                        }
                    }
                }
            }
            Console.WriteLine("-----");
            Console.WriteLine("Total number of locations: {0}.",cnt);
            Console.WriteLine("-----");
            foreach(var location in missingLocations)
            {
                Console.WriteLine("{0}, {1} -- {2}", location.Item1, location.Item2, location.Item3);
            }

            Console.WriteLine("-----");
            Console.WriteLine("Processing completed.");
            Console.ReadLine();

            // Get all indexed locations from database.
            // Look for location in first index.
            // If found look for the location in the second index.
            // If not found increment counter and display location id and name.
            // when finished display count.
        }

    }
}
