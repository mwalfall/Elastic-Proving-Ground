using Nest;

namespace JobIndexBuilder.Elasticsearch
{
    public interface IElasticsearchMappingResolver
    {
        PutMappingDescriptor<T> Resolve<T>(IConnectionSettingsValues connectionSettings) where T : class;
    }
}
