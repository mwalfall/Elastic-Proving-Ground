using Domain.ElasticsearchDocuments;
using Domain.Model;
using System.Collections.Generic;

namespace LocationIndexer.Services.Interfaces
{
    public interface IDuplicateSuggestionOutputResolverService
    {
        /// <summary>
        /// Set the Unique Names list.
        /// </summary>
        /// <param name="uniqueNames">List of LocationUniqueFormattedName objects</param>
        /// 
        void SetUniqueNames(List<LocationUniqueFormattedName> uniqueNames);

        /// <summary>
        /// Resolves duplicate names for the Suggest.Output
        /// </summary>
        /// <param name="esDocument">ElasticsearchLocation document</param>
        /// 
        ElasticsearchLocation Resolve(ElasticsearchLocation esDocument, string countryCode, string indexLanguage);
    }
}
