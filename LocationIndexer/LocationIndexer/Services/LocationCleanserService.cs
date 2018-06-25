using Domain.ElasticsearchDocuments;
using Domain.Enums;
using Domain.Model;
using LocationIndexer.Services.Interfaces;
using LocationIndexer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.Services
{
    public class LocationCleanserService : ILocationCleanserService
    {
        private List<LocationCleanserDirective> _locationCleanserDirectives = new List<LocationCleanserDirective>();

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adoClientService">IAdoClientService object</param>
        /// 
        public LocationCleanserService(List<LocationCleanserDirective> locationCleanserDirectives)
        {
            _locationCleanserDirectives = locationCleanserDirectives;
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Specifies if the submitted location is to be indexed based on rules.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public bool IsValidLocation(ElasticsearchLocation location)
        {
            if (string.IsNullOrWhiteSpace(location.CountryCode))
                return false;

            var countryCode = location.CountryCode;

            var locations = new List<ElasticsearchLocation> { location };

            switch (location.CountryCode.ToLower())
            {
                case "us":
                case "de":
                    return RemoveUnwantedUsLocations(locations, countryCode).Any();
                case "gb":
                    return RemoveUnwantedLocationsIncludeThirdFourthDivs(locations, countryCode).Any();
                default:
                    return DefaultRemoveUnwantedLocations(locations, countryCode).Any();
            }
        }

        /// <summary>
        /// Provides custom location cleansing on a per country basis.
        /// </summary>
        /// <param name="locations">List of LocationHierarchyView objects</param>
        /// <param name="countryCode">Country Code</param>
        /// 
        public List<ElasticsearchLocation> RemoveUnwantedLocations(List<ElasticsearchLocation> locations, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return locations;

            switch (countryCode.ToLower())
            {
                case "us":
                case "de":
                    return RemoveUnwantedUsLocations(locations, countryCode);
                case "gb":
                    return RemoveUnwantedLocationsIncludeThirdFourthDivs(locations, countryCode);
                default:
                    return DefaultRemoveUnwantedLocations(locations, countryCode);
            }
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// For the US remove locations containing historical, trailer park or mobile home.
        /// </summary>
        /// <param name="locations">List of LocationHierarchyView objects</param>
        /// <param name="countryCode">Country Code</param>
        /// 
        private List<ElasticsearchLocation> RemoveUnwantedUsLocations(IEnumerable<ElasticsearchLocation> locations, string countryCode)
        {
            var locationsToRemove =
                _locationCleanserDirectives.Where(
                    x => x.CountryCode.ToLower().Equals(countryCode.ToLower()) && x.Action == 0).Select(x => x.SourceId).ToList();

            var removalOverrides =
                _locationCleanserDirectives.Where(
                    x => x.CountryCode.ToLower().Equals(countryCode.ToLower()) && x.Action == 1).Select(x => x.SourceId).ToList();

            var filteredLocations = locations.ToList();

            if (locationsToRemove.Any())
                filteredLocations = filteredLocations.Where(l => (!locationsToRemove.Contains(l.ID))).ToList();

            if (filteredLocations.Any())
                filteredLocations = filteredLocations.Where(l =>
                    (removalOverrides.Any() && removalOverrides.Contains(l.ID)) ||
                    (
                        (l.TypeID == (int)LocationType.Country ||
                         l.TypeID == (int)LocationType.FirstOrderDivision ||
                         l.TypeID == (int)LocationType.SecondOrderDivision)
                        ||
                        (
                            l.TypeID == (int)LocationType.City &&
                            !l.City.ContainsDigits() &&
                            !l.City.ToLower().Contains("(historical)") &&
                            !l.City.ToLower().Contains("historical") &&
                            !l.City.ToLower().Contains("trailer park") &&
                            !l.City.ToLower().Contains("trailer park,") &&
                            !l.City.ToLower().Contains("mobile home park,") &&
                            !l.City.ToLower().Contains("mobile home,") &&
                            !l.City.ToLower().Contains("mobile home") &&
                            !l.City.ToLower().Contains("township 1") &&
                            !l.City.ToLower().Contains("township 2,") &&
                            !l.City.ToLower().Contains("township 3") &&
                            !l.City.ToLower().Contains("township 4") &&
                            !l.City.ToLower().Contains("township 5") &&
                            !l.City.ToLower().Contains("township 6") &&
                            !l.City.ToLower().Contains("township 7") &&
                            !l.City.ToLower().Contains("township 8") &&
                            !l.City.ToLower().Contains("township 9") &&
                            !l.City.ToLower().Contains("township 0") &&
                            !l.City.ToLower().Equals("township a") &&
                            !l.City.ToLower().Equals("township b") &&
                            !l.City.ToLower().Equals("township c") &&
                            !l.City.ToLower().Equals("township d") &&
                            !l.City.ToLower().Equals("township e") &&
                            !l.City.ToLower().Equals("township f") &&
                            !l.City.ToLower().Equals("township g") &&
                            !l.City.ToLower().Equals("township h") &&
                            !l.City.ToLower().Equals("township i") &&
                            !l.City.ToLower().Equals("township j") &&
                            !l.City.ToLower().Equals("township k") &&
                            !l.City.ToLower().Equals("township l") &&
                            !l.City.ToLower().Equals("township m") &&
                            !l.City.ToLower().Equals("township n") &&
                            !l.City.ToLower().Equals("township o") &&
                            !l.City.ToLower().Equals("township p") &&
                            !l.City.ToLower().StartsWith("exeter township prec")
                        )
                    )
            ).ToList();

            return filteredLocations;
        }

        /// <summary>
        /// Default location cleanser. Locations that are not to be indexed are removed from the submitted list
        /// of Location objects.
        /// </summary>
        /// <param name="locations">List of LocationHierarchyView objects</param>
        /// <param name="countryCode">Country Code</param>
        /// <returns></returns>
        private List<ElasticsearchLocation> DefaultRemoveUnwantedLocations(IEnumerable<ElasticsearchLocation> locations, string countryCode)
        {
            var locationsToRemove =
                _locationCleanserDirectives.Where(
                    x => x.CountryCode.ToLower().Equals(countryCode.ToLower()) && x.Action == 0).Select(x => x.SourceId).ToList();

            var removalOverrides =
               _locationCleanserDirectives.Where(
                   x => x.CountryCode.ToLower().Equals(countryCode.ToLower()) && x.Action == 1).Select(x => x.SourceId).ToList();

            var filteredLocations = locations.ToList();

            if (locationsToRemove.Any())
                filteredLocations = filteredLocations.Where(l => (!locationsToRemove.Contains(l.ID))).ToList();

            if (filteredLocations.Any())
                filteredLocations = filteredLocations.Where(location =>
                                            (removalOverrides.Any() && removalOverrides.Contains(location.ID)) ||
                                            (
                                                location.TypeID == (int)LocationType.Country ||
                                                location.TypeID == (int)LocationType.FirstOrderDivision ||
                                                location.TypeID == (int)LocationType.SecondOrderDivision
                                            ) ||
                                            (
                                                location.TypeID == (int)LocationType.City &&
                                                !location.City.ContainsDigits() &&
                                                !location.City.ToLower().Contains("historical") &&
                                                !location.City.ToLower().Contains("trailer park") &&
                                                !location.City.ToLower().Contains("trailer park,") &&
                                                !location.City.ToLower().Contains("mobile home park,") &&
                                                !location.City.ToLower().Contains("mobile home,") &&
                                                !location.City.ToLower().Contains("mobile home")
                                            )
                                        ).ToList();

            return filteredLocations;
        }

        /// <summary>
        /// Default location cleanser. Locations that are not to be indexed are removed from the submitted list
        /// of Location objects.
        /// </summary>
        /// <param name="locations">List of LocationHierarchyView objects</param>
        /// <param name="countryCode">Country Code</param>
        /// <returns></returns>
        private List<ElasticsearchLocation> RemoveUnwantedLocationsIncludeThirdFourthDivs(IEnumerable<ElasticsearchLocation> locations, string countryCode)
        {
            var locationsToRemove =
                _locationCleanserDirectives.Where(
                    x => x.CountryCode.ToLower().Equals(countryCode.ToLower()) && x.Action == 0).Select(x => x.SourceId).ToList();

            var removalOverrides =
               _locationCleanserDirectives.Where(
                   x => x.CountryCode.ToLower().Equals(countryCode.ToLower()) && x.Action == 1).Select(x => x.SourceId).ToList();

            var filteredLocations = locations.ToList();

            if (locationsToRemove.Any())
                filteredLocations = filteredLocations.Where(l => (!locationsToRemove.Contains(l.ID))).ToList();

            if (filteredLocations.Any())
                filteredLocations = filteredLocations.Where(location =>
                                            (removalOverrides.Any() && removalOverrides.Contains(location.ID)) ||
                                            (
                                                location.TypeID == (int)LocationType.Country ||
                                                location.TypeID == (int)LocationType.FirstOrderDivision ||
                                                location.TypeID == (int)LocationType.SecondOrderDivision ||
                                                location.TypeID == (int)LocationType.ThirdOrderDivision ||
                                                location.TypeID == (int)LocationType.FourthOrderDivision
                                            ) ||
                                            (
                                                location.TypeID == (int)LocationType.City &&
                                                !location.City.ContainsDigits() &&
                                                !location.City.ToLower().Contains("historical") &&
                                                !location.City.ToLower().Contains("trailer park") &&
                                                !location.City.ToLower().Contains("trailer park,") &&
                                                !location.City.ToLower().Contains("mobile home park,") &&
                                                !location.City.ToLower().Contains("mobile home,") &&
                                                !location.City.ToLower().Contains("mobile home")
                                            )
                                        ).ToList();

            return filteredLocations;
        }
        #endregion Private Methods
    }
}
