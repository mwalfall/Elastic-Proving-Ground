using Domain.ElasticsearchDocuments;
using Domain.Enums;
using Domain.Utilities;
using LocationIndexer.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.LocationBuilders
{
    public class ElasticsearchLocationBuilder
    {
        private ElasticsearchLocation _parentLocation = null;

        /// <summary>
        /// Returns a ElasticsearchLocation object.
        /// </summary>
        /// <param name="parentLocation">Parent ElasticsearchLocation object</param>
        /// <param name="locationType">LocationType enum</param>
        /// <param name="locationView">GeoLocation object</param>
        /// 
        public ElasticsearchLocation Build(LocationContext locationContext, GlobalContext globalContext)
        {
            var esDocument = BuildBaseObject(locationContext);

            globalContext.PreferredLocationNameService.SetPreferredName(esDocument, globalContext);
            SuggestionFormatService.SetSuggestion(esDocument, globalContext);
            globalContext.LocationSuggestionInputService.AddInputs(esDocument);
            globalContext.LocationAbbreviationService.AddAbbreviatedNames(esDocument);
            //globalContext.DuplicateSuggestionOutputResolverService.Resolve(esDocument);

            return esDocument;
        }

        #region Private Methods

        private ElasticsearchLocation BuildBaseObject(LocationContext locationContext)
        {
            var esLocation = new ElasticsearchLocation
            {
                Country = (locationContext.ParentLocation == null) ? locationContext.LocationView.Name : locationContext.ParentLocation.Country,
                CountryCode = (locationContext.ParentLocation == null) ? locationContext.LocationView.CountryCode : locationContext.ParentLocation.CountryCode,
                CountryCtx = GetCountryCtx(locationContext.LocationView),
                Geometry = new ElasticsearchGeometry { Latitude = (float)locationContext.LocationView.Latitude, Longitude = (float)locationContext.LocationView.Longitude },
                HierarchyPath = locationContext.LocationView.LocationPath,
                ID = locationContext.LocationView.Id,
                Population = SetPopulation(locationContext.LocationType, locationContext.LocationView),
                TypeID = (int)locationContext.LocationType
            };

            esLocation = SetDivisions(esLocation, locationContext.ParentLocation);

            esLocation = SetLocationName(esLocation, locationContext.LocationType, locationContext.LocationView);

            return esLocation;
        }
        private long SetPopulation(LocationType locationType, LocationView locationView)
        {
            if (locationView.Population > 0)
                return locationView.Population;

            switch(locationType)
            {
                case LocationType.Country:
                    return 0;
                case LocationType.FirstOrderDivision:
                    return 1;
                case LocationType.SecondOrderDivision:
                    return 2;
                case LocationType.ThirdOrderDivision:
                    return 3;
                case LocationType.FourthOrderDivision:
                    return 4;
                case LocationType.City:
                    return 5;
                default:
                    return locationView.Population;
            }
        }

        private ElasticsearchLocation SetLocationName(ElasticsearchLocation location, LocationType locationType, LocationView locationView)
        {
            switch(locationType)
            {
                case LocationType.Country:
                    location.Country = locationView.Name;
                    location.CountryCode = locationView.Code;
                    return location;

                case LocationType.FirstOrderDivision:
                    location.Division1 = locationView.Name;
                    location.Division1Code = locationView.Code;
                    return location;

                case LocationType.SecondOrderDivision:
                    location.Division2 = locationView.Name;
                    location.Division2Code = locationView.Code;
                    return location;

                case LocationType.ThirdOrderDivision:
                    location.Division3 = locationView.Name;
                    return location;

                case LocationType.FourthOrderDivision:
                    location.Division4 = locationView.Name;
                    return location;

                case LocationType.City:
                    location.City = locationView.Name;
                    return location;
                default:
                    return location;
            }
        }

        /// <summary>
        /// Populates the ElasticsearchLocation object with Division information.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object that is being constructed</param>
        /// <param name="parentLocation">Parent ElasticsearchLocation object</param>
        /// 
        private ElasticsearchLocation SetDivisions(ElasticsearchLocation location, ElasticsearchLocation parentLocation)
        {
            if (location.TypeID == (int)LocationType.Country)
                return location;

            location.Division1Code = parentLocation.Division1Code;
            location.Division1 = parentLocation.Division1;
            location.Division2Code = parentLocation.Division2Code;
            location.Division2 = parentLocation.Division2;
            location.Division3 = parentLocation.Division3;
            location.Division4 = parentLocation.Division4;

            return location;
        } 

        /// <summary>
        /// Returns the Country Context information for an ElaasticsearchLocation object
        /// </summary>
        /// <param name="geoLocation">GeoLocation object</param>
        /// 
        private List<string> GetCountryCtx(LocationView geoLocation)
        {
            return new List<string>
            {
                "all",
                geoLocation.CountryCode,
                "all-Country",
                string.Format("{0}-Country", geoLocation.CountryCode)
            };
        }

        /// <summary>
        /// Get the parent object for the provided GeoLocation object.
        /// </summary>
        /// <param name="locations">List of ElasticsearchLocation objects</param>
        /// <param name="locationType">LocationType enum</param>
        /// <param name="locationView">LocationView object</param>
        /// 
        private ElasticsearchLocation GetParentLocation(List<ElasticsearchLocation> locations, LocationType locationType, LocationView locationView)
        {
            if (locationType == LocationType.Country)
                return null;

            var items = locationView.ParentLocationPath.Split('/');
            var index = items.Length - 2;
            var id = Convert.ToUInt32(items[index]);

            if (_parentLocation != null && _parentLocation.ID == id)
                return _parentLocation;

            _parentLocation = locations.SingleOrDefault(x => x.ID == id);

            return _parentLocation;           
        }
        #endregion Private Methods
    }
}
