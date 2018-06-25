using Domain.ElasticsearchDocuments;
using Domain.Enums;
using Domain.Utilities;

namespace LocationIndexer.LocationBuilders
{
    public class LocationContext
    {
        public ElasticsearchLocation ParentLocation { get; set; }
        public LocationType LocationType { get; set; }
        public LocationView LocationView { get; set; }
    }
}
