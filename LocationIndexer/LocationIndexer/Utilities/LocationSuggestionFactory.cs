using Domain.ElasticsearchDocuments;
using Domain.Enums;
using Domain.Model;
using System;
using System.Collections.Generic;

namespace LocationIndexer.Utilities
{
    public class LocationSuggestionFactory
    {
        /// <summary>
        /// Create Location Suggestion object.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// <param name="locationAliases">List of LocationAlias objects</param>
        /// 
        public static ElasticsearchLocation.Suggestion Create(ElasticsearchLocation location, List<LocationAlias> locationAliases, string languageCode)
        {
            switch (GetLocationType(location))
            {
                case LocationType.Country:
                    return new CountryLocationSuggestion(location).ToDocument();

                case LocationType.FirstOrderDivision:
                    return new FirstOrderDivisionLocationSuggestion(location).ToDocument();

                // Language used by LocationNameUtility class to ensure abbreviation for location names not used with ZH.
                case LocationType.SecondOrderDivision:
                    return new SecondOrderDivisionLocationSuggestion(location, languageCode).ToDocument();

                case LocationType.ThirdOrderDivision:
                    return new ThirdOrderDivisionLocationSuggestion(location).ToDocument();

                case LocationType.FourthOrderDivision:
                    return new FourthOrderDivisionLocationSuggestion(location).ToDocument();

                // Language used by LocationNameUtility class to ensure abbreviation for location names not used with ZH.
                case LocationType.City:
                    return new CityLocationSuggestion(location, languageCode).ToDocument();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Determine if Earth is being processed (TypeID = -1)
        /// If Earth is being processed set location type to Unknown.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        private static LocationType GetLocationType(ElasticsearchLocation location)
        {
            return (location.TypeID == -1) ? LocationType.Unknown : (LocationType)location.TypeID;
        }
    }
}
