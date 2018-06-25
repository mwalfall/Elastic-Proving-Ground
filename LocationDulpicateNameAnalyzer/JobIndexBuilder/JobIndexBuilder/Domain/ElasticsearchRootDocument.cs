using Nest;

namespace JobIndexBuilder.Domain
{
    public abstract class ElasticsearchRootDocument
    {
        [ElasticProperty(Name = "id")]
        public long ID { get; set; }
    }
}
