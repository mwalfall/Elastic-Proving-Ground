using Domain.ElasticsearchDocuments;
using System.Collections.Generic;

namespace LocationIndexer.Services.Interfaces
{
    public interface ILocationCleanserService
    {
        /// <summary>
        /// Specifies if the submitted location is to be indexed based on rules.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        bool IsValidLocation(ElasticsearchLocation location);

        /// <summary>
        /// Provides custom location cleansing on a per country basis.
        /// </summary>
        /// <param name="locations">List of LocationHierarchyView objects</param>
        /// <param name="countryCode">Country Code</param>
        /// 
        List<ElasticsearchLocation> RemoveUnwantedLocations(List<ElasticsearchLocation> locations, string countryCode);
    }
}
