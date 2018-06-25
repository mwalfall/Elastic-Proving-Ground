using Domain.ElasticsearchDocuments;
using Domain.Model;
using LocationIndexer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LocationIndexer.Services
{
    public class LocationAbbreviationService : ILocationAbbreviationService
    {
        private List<LocationAbbreviation> _abbreviationMapping;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="abbreviationMapping">List of LocationAbbreviation objects</param>
        /// 
        public LocationAbbreviationService(List<LocationAbbreviation> abbreviationMapping)
        {
            _abbreviationMapping = abbreviationMapping;
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Returns a list of abbreviated names for a location.
        /// NOTE: To reduce execution time 'Air Force Base' is only checked for locations in the US. 
        /// Analysis of the Location table shows that the string 'Air Force Base' is only used in the US.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public ElasticsearchLocation AddAbbreviatedNames(ElasticsearchLocation location)
        {
            if (location.TypeID != 5)
                return location;

            var abbreviations = GetAbbreviatedNames(location.City, location.CountryCode);
            if (!abbreviations.Any())
                return location;

            var suggestionInputs = location.Suggest.Input.ToList();
            suggestionInputs.AddRange(abbreviations);
            location.Suggest.Input = suggestionInputs.Distinct().ToList();

            return location;
        }

        /// <summary>
        /// Returns a list of abbreviated names for a location.
        /// NOTE: To reduce execution time 'Air Force Base' is only checked for locations in the US. 
        /// Analysis of the Location table shows that the string 'Air Force Base' is only used in the US.
        /// </summary>
        /// <param name="name">Name of location</param>
        /// <param name="countryCode">Country Code</param>
        /// <returns>List of abbreviated names for the submitted location name</returns>
        public List<string> GetAbbreviatedNames(string name, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<string>();

            foreach (var mapping in _abbreviationMapping)
            {
                if (mapping.Name.ToLower().Equals("air force base"))
                {
                    if (!countryCode.ToLower().Equals("us"))
                        continue;

                    if (Regex.IsMatch(name, mapping.RegexString, RegexOptions.IgnoreCase))
                        return GetAbbreviatedNames(name, mapping);
                }
                else
                {
                    if (Regex.IsMatch(name, mapping.RegexString, RegexOptions.IgnoreCase))
                        return GetAbbreviatedNames(name, mapping);
                }
            }

            return new List<string>();
        }
        #endregion Private Methods

        #region Private Methods

        /// <summary>
        /// Get the abbreviated names for a submitted location name.
        /// </summary>
        /// <param name="name">Name of the location</param>
        /// <param name="abbreviationMapping">Object of type LocationAbbreviation</param>
        /// <returns>List of abbreviated names for the submitted location name</returns>
        /// 
        private List<string> GetAbbreviatedNames(string name, LocationAbbreviation abbreviationMapping)
        {
            var indexOf = name.ToLower().IndexOf(abbreviationMapping.Name.ToLower(), StringComparison.Ordinal);
            var truncatedName = name.Remove(indexOf, abbreviationMapping.Name.Length);
            var abbreviations = abbreviationMapping.Abbreviations.Split(',');

            var abbreviationList = new List<string>();

            foreach (var abbreviation in abbreviations)
            {
                abbreviationList.Add(truncatedName.Substring(0, indexOf) + abbreviation + truncatedName.Substring(indexOf));
            }

            return abbreviationList;
        }
        #endregion Private Methods
    }
}
