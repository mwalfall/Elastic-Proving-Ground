using Nest;

namespace LocationDulpicateNameAnalyzer.Model
{
    public abstract class ElasticsearchRootDocument
    {
        [ElasticProperty(Name = "id")]
        public long ID { get; set; }
    }
}
