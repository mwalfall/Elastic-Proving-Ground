using Elasticsearch.Net.ConnectionPool;
using LocationDulpicateNameAnalyzer.Model;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationDulpicateNameAnalyzer.Services
{
    public class NamingService
    {
        private AdoClientService _adoClientService;

        public NamingService(AdoClientService adoClientService)
        {
            _adoClientService = adoClientService;
        }

        public void SetUniqueNames(string languageCode, string countryCode = "all")
        {
            var duplicateLocations = _adoClientService.GetDuplicateLocations(languageCode, countryCode);

            if (duplicateLocations.Any())
            {
                

                var service = new ElasticClientService();
                var client = service.GetClient();

                foreach(var location in duplicateLocations)
                {
                    var result = client.Get<ElasticsearchLocation>(g => g.Index("locations-en-05.05.16.09.24.09").Type("location").Id(location.Id));

                    if (result.Found && result.Source != null && result.Source.Suggest != null && result.Source.Suggest.Output != null)  
                        location.Name = result.Source.Suggest.Output;
                }
                try
                {
                    _adoClientService.SetDuplicateLocations(duplicateLocations, DataTableBuilder.GetLocationUniqueFormattedNameTable());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }          
        }
    }
}
