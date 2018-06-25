using Domain.ElasticsearchDocuments;
using Domain.Model;
using LocationIndexer.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.Services
{
    public class DuplicateSuggestionOutputResolverService : IDuplicateSuggestionOutputResolverService
    {
        private List<LocationUniqueFormattedName> _uniqueNames = new List<LocationUniqueFormattedName>();

        /// <summary>
        /// Set the Unique Names list.
        /// </summary>
        /// <param name="uniqueNames">List of LocationUniqueFormattedName objects</param>
        /// 
        public void SetUniqueNames(List<LocationUniqueFormattedName> uniqueNames)
        {
            _uniqueNames = uniqueNames;
        }

        /// <summary>
        /// Resolves duplicate names for the Suggest.Output
        /// </summary>
        /// <param name="esDocument">ElasticsearchLocation document</param>
        /// 
        public ElasticsearchLocation Resolve(ElasticsearchLocation esDocument, string countryCode, string indexLanguage)
        {
            if (esDocument.TypeID == 0 || esDocument.TypeID == 1)
                return esDocument;

            var uniqueName = _uniqueNames.SingleOrDefault(x => x.Id == esDocument.ID && x.CountryCode.ToLower().Equals(countryCode.ToLower()) && x.IndexLanguage.ToLower().Equals(indexLanguage.ToLower()));
            if (uniqueName == null)
                return esDocument;

            esDocument.FormattedName = uniqueName.Name;
            esDocument.Suggest.Output = uniqueName.Name;

            return esDocument;
        }
    }
}
