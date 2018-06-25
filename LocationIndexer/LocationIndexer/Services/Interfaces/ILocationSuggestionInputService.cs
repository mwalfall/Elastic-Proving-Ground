using Domain.ElasticsearchDocuments;
using System.Collections.Generic;

namespace LocationIndexer.Services.Interfaces
{
    public interface ILocationSuggestionInputService
    {
        /// <summary>
        /// Takes in a location object and adds suggestions to Suggest.Input list.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        ElasticsearchLocation AddInputs(ElasticsearchLocation location);

        /// <summary>
        /// Get the input suggestions for the specified source id.
        /// </summary>
        /// <param name="sourceId">Source Id</param>
        /// 
        List<string> GetSuggestions(long sourceId);

        /// <summary>
        /// Transform characters so that locations can be found using default letters.
        /// For example ö -> oe, ü -> ue.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="locationName">Location Name</param>
        /// <returns>Transformed name for the location. Otherwise, mptry string if transformation did not occur.</returns>
        /// 
        string GetSuggestionWithCharacterTransformation(string countryCode, string locationName);
    }
}
