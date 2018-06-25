using Domain.ElasticsearchDocuments;
using Domain.Enums;
using Domain.Model;
using Domain.Utilities;
using LocationIndexer.LocationBuilders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer
{
    public class LocationProcessor
    {
        private GlobalContext _globalContext;
        private ElasticsearchLocation _parentLocation;
        private long _previousId;

        private List<LocationType> _locationTypes = new List<LocationType>
        {
            LocationType.Country,
            LocationType.FirstOrderDivision,
            LocationType.SecondOrderDivision,
            LocationType.ThirdOrderDivision,
            LocationType.FourthOrderDivision,
            LocationType.City
        };

        #region Contructor
        public LocationProcessor(GlobalContext globalContext)
        {
            _globalContext = globalContext;
        }
        #endregion Constructor

        public void Process()
        {
            var esLocationBuilder = new ElasticsearchLocationBuilder();
            var locations = new List<ElasticsearchLocation>();

            var cnt = 1;
            Console.WriteLine(string.Format("**** Processing starts: {0}", DateTime.Now.ToLongTimeString()));
            Console.WriteLine("");

            foreach (var country in _globalContext.EnvironmentContext.CountryGroups)
            { 
                cnt = 1;
                Console.WriteLine(string.Format("**** Country being Processing: {0}. -------------------------------", country));

                _globalContext.LocationAliases = _globalContext.AdoClientService.GetLocationAliases(country, _globalContext.EnvironmentContext.LanguageCodes);
                _globalContext.DuplicateSuggestionOutputResolverService.SetUniqueNames(new List<LocationUniqueFormattedName>());

                foreach (var locationType in _locationTypes)
                {
                    Console.WriteLine(string.Format("**** Processing: {0}.", locationType.ToString()));
                    
                    var query = PlatformQueryParameters.GetQuery(locationType, country);
                    var locationViews = _globalContext.AdoClientService.GetLocations(query);

                    // Build Location
                    Console.WriteLine(string.Format("** Build starts: {0}", DateTime.Now.ToLongTimeString()));
                    foreach (var locationView in locationViews)
                    {
                        var parentLocation = GetParentLocation(locations, locationType, locationView);

                        var locationContext = new LocationContext
                        {
                            ParentLocation = parentLocation,
                            LocationType = locationType,
                            LocationView = locationView
                        };

                        locations.Add( esLocationBuilder.Build(locationContext, _globalContext) );
                        Console.Write(string.Format("\r** Documents built {0}.", cnt++));
                    }
                    Console.WriteLine(string.Format("\r** Documents processed: {0}.", --cnt));
                    Console.WriteLine(string.Format("** Build ends: {0}", DateTime.Now.ToLongTimeString()));
                    Console.WriteLine("");
                }

                var locationsToBeIndexed = _globalContext.LocationCleanserService.RemoveUnwantedLocations(locations, country);
                _globalContext.XmlDataService.SetToXml<ElasticsearchLocation>(locationsToBeIndexed, string.Format("Location{0}.xml",country.ToUpper()), _globalContext.EnvironmentContext.XmlDocumentsDestinationPath);
            }
            Console.Write(string.Format("**** Processing ended: {0}", DateTime.Now.ToLongTimeString()));
            Console.ReadLine();
        }

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="locationType"></param>
        /// <param name="locationView"></param>
        /// <returns></returns>
        private ElasticsearchLocation GetParentLocation(List<ElasticsearchLocation> locations, LocationType locationType, LocationView locationView)
        {
            if (locationType == LocationType.Country)
                return null;

            if (_previousId == locationView.Id)
                return _parentLocation;

            _previousId = GetParentId(locationView);
            _parentLocation = locations.SingleOrDefault(x => x.ID == _previousId);

            return _parentLocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="locationType"></param>
        /// <param name="geoLocation"></param>
        /// <returns></returns>
        private ElasticsearchLocation GetParentLocationOfCity(List<ElasticsearchLocation> locations, LocationType locationType, GeoLocation geoLocation)
        {
            var parent = locations.SingleOrDefault(x => x.Division3Code == geoLocation.Admin3Code && x.TypeID == (int)LocationType.ThirdOrderDivision);
            if (parent != null)
                return parent;

            parent = locations.SingleOrDefault(x => x.Division2Code == geoLocation.Admin2Code && x.TypeID == (int)LocationType.SecondOrderDivision);
            if (parent != null)
                return parent;

            parent = locations.SingleOrDefault(x => x.Division1Code == geoLocation.Admin1Code && x.TypeID == (int)LocationType.FirstOrderDivision);
            if (parent != null)
                return parent;

            parent = locations.SingleOrDefault(x => x.CountryCode == geoLocation.CountryCode && x.TypeID == (int)LocationType.Country);
            if (parent != null)
                return parent;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationView"></param>
        /// <returns></returns>
        private int GetParentId(LocationView locationView)
        {
            var parentPath = locationView.ParentLocationPath.Split('/');

            var item = parentPath.Length - 2;
            return Convert.ToInt32(parentPath[item]);
        }
        #endregion Private Methods
    }
}
