using LocationDulpicateNameAnalyzer.Model;
using System;
using System.Collections.Generic;

namespace LocationDulpicateNameAnalyzer.Services
{
    public class DuplicateLocationMappingResolver
    {
        private AdoClientService _adoClientService;
        public DuplicateLocationMappingResolver(AdoClientService adoClientService)
        {
            _adoClientService = adoClientService;
        }

        public void Resolve()
        {
            
            LocationHierarchyAnalysis previousProcessedLocation = null;
            LocationHierarchyAnalysis indexedLocation = null;

            var pName = string.Empty;
            var cnt = 0;
            var uCnt = 0;

            Console.WriteLine("STARTING...");

            var countries = _adoClientService.GetCountryCodes();

            foreach (var country in countries)
            {
                Console.WriteLine(string.Format("Country being processed: {0}.", country));
                
                var notMappedDuplicateLocations = _adoClientService.GetDuplicatesWithoutMappedToId(country);
                Console.WriteLine(string.Format("Unmapped locations: {0}.", notMappedDuplicateLocations.Count));

                foreach(var location in notMappedDuplicateLocations)
                {
                    cnt++;

                    if (previousProcessedLocation == null || !previousProcessedLocation.PName.Equals(location.PName))
                    {
                        if (location.PName.Contains("'"))
                            pName = location.PName.Replace("'", "''");
                        else
                            pName = location.PName;

                        indexedLocation = _adoClientService.GetIndexedHierarchyAnalysis(location.CountryCode, pName);
                        if (indexedLocation != null)
                        {
                            uCnt++;
                            _adoClientService.UpdateHierarchyAnalysis(location.Id, indexedLocation.Id);
                            previousProcessedLocation = indexedLocation;
                        }
                        else
                            Console.WriteLine("-------------- > NOT INDEXED: {0}, {1}", location.Id, location.PName);
                    }
                    else
                    {
                        uCnt++;
                        _adoClientService.UpdateHierarchyAnalysis(location.Id, previousProcessedLocation.Id);
                    }
                }
            }
            Console.WriteLine("{0} locations processed. {1} updated.", cnt, uCnt);
            Console.WriteLine("FINISHED...");
        }
    }
}
