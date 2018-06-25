using Domain.Model;
using Domain.Utilities;
using System;
using System.Collections.Generic;

namespace LocationIndexer.Services.Interfaces
{
    public interface IAdoClientService
    {
        /// <summary>
        /// Gets the Characters used to add suggestions for certain locations.
        /// For example: 'söc' would have a suggestion.input 'soec'.
        /// </summary>
        /// 
        List<CharacterTransformation> GetCharacterTransformations();

        /// <summary>
        /// Get the list of countries codes.
        /// </summary>
        /// 
        List<string> GetCountryCodes();

        /// <summary>
        /// Returns the feature codes for the submitted country codes
        /// </summary>
        /// <returns>List of Tuples(CountryCode,FeatureCode)</returns>
        /// 
        List<Tuple<string, string>> GetFeatureCodes();

        /// <summary>
        /// Returns the abbreviations for locations. Example: Fort to Ft, Saint to St.
        /// </summary>
        /// 
        List<LocationAbbreviation> GetLocationAbbreviations();

        /// <summary>
        /// Returns a list of LocationCleanserDirective objects. 
        /// These objects indicate locations that are to be deleted or not deleted from the index.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// 
        List<LocationCleanserDirective> GetLocationCleanserDirectives(string countryCode);

        /// <summary>
        /// Returns a list of LocationCleanserDirective objects. 
        /// These objects indicate locations that are to be deleted or not deleted from the index.
        /// </summary>
        /// 
        List<LocationCleanserDirective> GetLocationCleanserDirectives();

        /// <summary>
        /// Returns the suggestions for locations. Alternate names that are used locally but not found in GeoNames dataset. 
        /// Example: Local names for military bases.
        /// </summary>
        /// 
        List<LocationSuggestionInput> GetLocationSuggestionInputs();

        /// <summary>
        /// Get the custom names for a location. This data is used in situations where it is not possible to programmatically
        /// obtain the preferred name for a location.
        /// </summary>
        /// 
        List<LocationName> GetLocationNames();

        /// <summary>
        /// Returns the preferred names directives for locations. These objects indicate where to find the preferred name for a specified location.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// 
        List<PreferredLocationNameSelection> GetPreferredLocationNameSelections(string countryCode = null);

        /// <summary>
        /// Get location alias objects
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="languageCodes">Language code</param>
        /// 
        List<LocationAlias> GetLocationAliases(string countryCode, List<string> languageCodes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<LocationView> GetLocations(string query);

        /// <summary>
        /// Get unique location names
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="languageCode">Language Code</param>
        /// 
        List<LocationUniqueFormattedName> LocationUniqueFormattedName(string countryCode, string languageCode);

        /// <summary>
        /// Get multiple resultsets.
        /// </summary>
        /// 
        List<Tuple<string, object>> GetMultiResultSets();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="languageCodes">Language Codes</param>
        /// <param name="languageCode">Language Code</param>
        /// <returns></returns>
        List<Tuple<string, object>> GetAliasesAndUniqueNames(string countryCode, List<string> languageCodes, string languageCode);
    }
}
