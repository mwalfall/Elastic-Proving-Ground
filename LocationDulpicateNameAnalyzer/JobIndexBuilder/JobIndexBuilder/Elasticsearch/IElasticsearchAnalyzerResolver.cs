using Nest;

namespace JobIndexBuilder.Elasticsearch
{
    public interface IElasticsearchAnalyzerResolver
    {
        CreateIndexDescriptor Resolve<T>(CreateIndexDescriptor descriptor) where T : class;
    }
}
