using JobIndexBuilder.Domain;
using Nest;
using System.Collections.Generic;

namespace JobIndexBuilder.Elasticsearch
{
    public class ElasticsearchAnalyzerResolver : IElasticsearchAnalyzerResolver
    {
        public CreateIndexDescriptor Resolve<T>(CreateIndexDescriptor descriptor) where T : class
        {
            var type = typeof(T);

            return type == typeof(ElasticsearchCategory)
            ? AddCategoryAnalyzers(descriptor)
            : descriptor;
        }

        private CreateIndexDescriptor AddCategoryAnalyzers(CreateIndexDescriptor descriptor)
        {
            var autoComplete = new CustomAnalyzer
            {
                Filter = new List<string> { "lowercase", "asciifolding", "autocomplete_filter" },
                Tokenizer = "standard"
            };

            var autoCompleteNative = new CustomAnalyzer
            {
                Filter = new List<string> { "lowercase", "autocomplete_filter" },
                Tokenizer = "standard"
            };

            descriptor.Analysis(x => x
                .TokenFilters(f => f
                    .Add("autocomplete_filter", new EdgeNGramTokenFilter { MaxGram = 20, MinGram = 1 }))
                .Analyzers(a => a
                   .Add("autocomplete", autoComplete)
                   .Add("autocompletenative", autoCompleteNative)));

            return descriptor;
        }
    }
}
