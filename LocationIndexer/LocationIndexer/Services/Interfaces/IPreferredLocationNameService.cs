using Domain.ElasticsearchDocuments;
using LocationIndexer.LocationBuilders;

namespace LocationIndexer.Services.Interfaces
{
    public interface IPreferredLocationNameService
    {
        /// <summary>
        /// Entrypoint to determine the name to use for a location. The ordering is (short-circuit) as follows: 
        /// Custom Name - Short Name - Preferred Name - General Name (in specified language) - Default Name
        /// </summary>
        /// <param name="esDocument">ElasticsearchLocatio object</param>
        /// <param name="globalContext">GlobalContext object</param>
        /// 
        ElasticsearchLocation SetPreferredName(ElasticsearchLocation esDocument, GlobalContext globalContext);
    }
}
