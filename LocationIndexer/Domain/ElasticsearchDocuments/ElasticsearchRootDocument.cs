using Nest;

namespace Domain.ElasticsearchDocuments
{
    public abstract class ElasticsearchRootDocument
    {
        [ElasticProperty(Name = "id")]
        public long ID { get; set; }
    }
}
