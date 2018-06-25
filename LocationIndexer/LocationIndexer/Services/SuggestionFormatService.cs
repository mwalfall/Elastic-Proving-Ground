using Domain.ElasticsearchDocuments;
using Domain.Model;
using LocationIndexer.LocationBuilders;
using LocationIndexer.Utilities;
using System.Collections.Generic;

namespace LocationIndexer.Services
{
    public class SuggestionFormatService
    {
        /// <summary>
        /// Entrypoint to create the suggestion object for the submitted ES location object.
        /// </summary>
        /// <param name="esDocument">ElasticsearchLocation object</param>
        /// <param name="globalContext">GlobalContex object</param>
        /// 
        public static ElasticsearchLocation SetSuggestion(ElasticsearchLocation esDocument, GlobalContext globalContext)
        {
            esDocument = ProcessSuggest(esDocument, globalContext.LocationAliases, globalContext.EnvironmentContext.IndexLanguage);
            esDocument = SetFormattedName(esDocument);

            return esDocument;
        }

        #region Private Methods

        /// <summary>
        /// Factory object that orchestras the suggestion object creation process.
        /// </summary>
        /// <param name="esDocument">ElasticsearchLocation object</param>
        /// <param name="alternateNames">List of LocationAlias objects</param>
        /// <param name="languageCode">Language code</param>
        /// 
        private static ElasticsearchLocation ProcessSuggest(ElasticsearchLocation esDocument, List<LocationAlias> locationAliases, string languageCode)
        {
            esDocument.Suggest = LocationSuggestionFactory.Create(esDocument, locationAliases, languageCode);
            return esDocument;
        }

        /// <summary>
        /// Sets the Formatted Name for the location object to the value contained in the Suggest.Output property.
        /// </summary>
        /// <param name="esDocument">ElasticsearchLocation</param>
        /// 
        private static ElasticsearchLocation SetFormattedName(ElasticsearchLocation esDocument)
        {
            esDocument.FormattedName = esDocument.Suggest.Output;
            return esDocument;
        }
        #endregion Private Methods
    }
}
