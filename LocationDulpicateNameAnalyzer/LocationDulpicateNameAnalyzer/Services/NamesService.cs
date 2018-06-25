using System;
using System.Linq;
using LocationDulpicateNameAnalyzer.Model;
using System.Collections.Generic;
using System.Configuration;

namespace LocationDulpicateNameAnalyzer.Services
{
    class NamesService
    {
        private AdoClientService _adoClientService;

        public NamesService(AdoClientService adoClientService)
        {
            _adoClientService = adoClientService;
        }

        public void Analyze()
        {
            var processSelectedCountries = ConfigurationManager.AppSettings["ProcessSelectedCountries"];
            var countryCodes = (processSelectedCountries.ToLower().Equals("true"))
                ? ConfigurationManager.AppSettings["Countries"].Split(',').ToList()
                : _adoClientService.GetCountryCodes();
           
            if (countryCodes.Any())
            {
                var service = new ElasticClientService();
                var client = service.GetClient();

                foreach (var countryCode in countryCodes)
                {
                    Console.WriteLine(String.Format("--> Country being processed: {0}", countryCode));

                    var locationIds = _adoClientService.GetLocationIds(countryCode);
                    var formattedNames = new List<LocationFormattedNamesAnalysis>();
                    if(locationIds.Any())
                    {
                        foreach(var id in locationIds)
                        {
                            var result = client.Get<ElasticsearchLocation>(g => g.Index("locations-07.20.16.15.44.58").Type("location").Id(id));

                            if (result.Found && result.Source != null && result.Source.Suggest != null && result.Source.Suggest.Output != null)
                            {
                                formattedNames.Add(new LocationFormattedNamesAnalysis
                                {
                                    ID = result.Source.Suggest.Payload.ID,
                                    CountryCode = result.Source.Suggest.Payload.CountryCode,
                                    LanguageCode = "en",
                                    FormattedName = result.Source.Suggest.Output,
                                    FormattedNameNon = result.Source.Suggest.Output,
                                    TypeId = result.Source.Suggest.Payload.TypeID,
                                    Formatting = 0
                                });
                            }   
                        }    
                    }
                    try
                    {
                        _adoClientService.FormattedNameForAnalysis(formattedNames, DataTableBuilder.GetFormattedNamesAnalysisTable());
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
}
