using Domain.ElasticsearchDocuments;
using System.Collections.Generic;

namespace LocationIndexer.Services.Interfaces
{
    public interface ILocationAbbreviationService
    {
        /// <summary>
        /// Returns a list of abbreviated names for a location.
        /// NOTE: To reduce execution time 'Air Force Base' is only checked for locations in the US. 
        /// Analysis of the Location table shows that the string 'Air Force Base' is only used in the US.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        ElasticsearchLocation AddAbbreviatedNames(ElasticsearchLocation location);

        /// <summary>
        /// Returns a list of abbreviated names for a location.
        /// NOTE: To reduce execution time 'Air Force Base' is only checked for locations in the US. 
        /// Analysis of the Location table shows that the string 'Air Force Base' is only used in the US.
        /// </summary>
        /// <param name="name">Name of location</param>
        /// <param name="countryCode">Country Code</param>
        /// <returns>List of abbreviated names for the submitted location name</returns>
        List<string> GetAbbreviatedNames(string name, string countryCode);
    }
}
