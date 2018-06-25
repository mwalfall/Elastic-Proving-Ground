using JobIndexBuilder.Domain;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobIndexBuilder.Elasticsearch
{
    public class ElasticsearchAnalysisResolver : IElasticsearchAnalysisResolver
    {
        public AnalysisDescriptor Resolve<T>(LanguageCode languageCode = LanguageCode.EN)
        {
            AnalysisDescriptor desciptor = null;
            if (typeof(T) == typeof(ElasticsearchJob))
            {
                desciptor = GetJobsAnalysisDescriptor(languageCode);
            }

            return desciptor;
        }

        /// <summary>
        /// Job Analysis descriptions
        /// </summary>
        /// 
        private AnalysisDescriptor GetJobsAnalysisDescriptor(LanguageCode languageCode = LanguageCode.EN)
        {
            var descriptor = new AnalysisDescriptor();
            descriptor.TokenFilters(cf => cf.Add("shingle_title", new ShingleTokenFilter()));

            descriptor.TokenFilters(
                f => f.Add("job_stopfilter", new StopTokenFilter { Stopwords = new List<string> { "job", "jobs" } }));

            // Title Analyzer
            var titleAnalyzer = GetTitleAnalyzer(languageCode);
            descriptor.Analyzers(a => a.Add("job_title", titleAnalyzer));

            // Path Analyzer
            var pathAnalyzer = GetPathAnalyzer();
            descriptor.Analyzers(a => a.Add("path", pathAnalyzer));

            // Lowercase Analyzer
            var lowercaseAnalyzer = GetLowercaseAnalyzer(languageCode);
            descriptor.Analyzers(a => a.Add("lowercase", lowercaseAnalyzer));

            // Snowball Token Filter
            var snowballPorterFilter = GetStemmerTokenFilter(languageCode);
            descriptor.TokenFilters(d => d.Add("snowballPorterFilter", snowballPorterFilter));

            // Stopwords Filter
            var stopwordFilter = GetStopwordFilter(languageCode);
            descriptor.TokenFilters(d => d.Add("stopwordFilter", stopwordFilter));

            // Word Delimiter Token Filter
            var wdtFitler = GetWordDelimeterTokenFilter(languageCode);
            descriptor.TokenFilters(d => d.Add("wdtFitler", wdtFitler));

            // Job Default Analyzer
            var jobDefaultAnalyzer = GetJobDefaultAnanyzer(languageCode);
            descriptor.Analyzers(a => a.Add("jobDefaultAnalyzer", jobDefaultAnalyzer));

            // Job Default with Delimiter Analyzer
            var jobDefaultWithDelimiterAnalyzer = GetJobDefaultWithDelimiterAnalyzer(languageCode);
            descriptor.Analyzers(a => a.Add("jobDefaultWithDelimiterAnalyzer", jobDefaultWithDelimiterAnalyzer));

            // Title Suggestion Anlyzer
            var titleSuggestAnalyzer = GetTitleSuggestAnalyzer(languageCode);
            descriptor.Analyzers(a => a.Add("titleSuggestAnalyzer", titleSuggestAnalyzer));

            // country, match first node in hierarchy path
            descriptor.Tokenizers(t => t.Add("country_path", new PatternTokenizer { Pattern = "^(/[0-9]+/).*", Group = 1 }));
            descriptor.Analyzers(a => a.Add("country_path", new CustomAnalyzer { Tokenizer = "country_path" }));

            // region, match first and second nodes in hierarchy path
            descriptor.Tokenizers(t => t.Add("region_path", new PatternTokenizer { Pattern = "^(/[0-9]+/[0-9]+/).*", Group = 1 }));
            descriptor.Analyzers(a => a.Add("region_path", new CustomAnalyzer { Tokenizer = "region_path" }));

            // city, match first four or first three nodes in path as cities in some countries lack a second level division
            descriptor.Tokenizers(t => t.Add("city_path", new PatternTokenizer { Pattern = "^(/[0-9]+/[0-9]+/[0-9]+/[0-9]+/[0-9]+/[0-9]+/|/[0-9]+/[0-9]+/[0-9]+/[0-9]+/[0-9]+/|/[0-9]+/[0-9]+/[0-9]+/[0-9]+/|/[0-9]+/[0-9]+/[0-9]+/).*", Group = 1 }));
            descriptor.Analyzers(a => a.Add("city_path", new CustomAnalyzer { Tokenizer = "city_path" }));

            return descriptor;
        }

        private CustomAnalyzer GetTitleAnalyzer(LanguageCode languageCode)
        {
            switch (languageCode)
            {
                case LanguageCode.ZH:
                    return new CustomAnalyzer
                    {
                        Tokenizer = "standard",
                        Filter = new string[] { "cjk_width", "cjk_bigram", "standard", "lowercase", "stop", "job_stopfilter", "shingle_title", "asciifolding" }
                    };
                default:
                    return new CustomAnalyzer
                    {
                        Tokenizer = "standard",
                        Filter = new string[] { "standard", "lowercase", "stop", "job_stopfilter", "shingle_title", "asciifolding" }
                    };
            }
        }

        private CustomAnalyzer GetPathAnalyzer()
        {
            return new CustomAnalyzer()
            {
                Tokenizer = "path_hierarchy"
            };
        }

        private CustomAnalyzer GetLowercaseAnalyzer(LanguageCode languageCode)
        {
            return new CustomAnalyzer
            {
                Tokenizer = "keyword",
                Filter = new string[] { "lowercase", "asciifolding" }
            };
        }

        private StemmerTokenFilter GetStemmerTokenFilter(LanguageCode languageCode)
        {
            switch (languageCode)
            {
                case LanguageCode.EN:
                    return new StemmerTokenFilter { Language = "english" };
                case LanguageCode.FR:
                    return new StemmerTokenFilter { Language = "french" };
                case LanguageCode.ES:
                    return new StemmerTokenFilter { Language = "spanish" };
                case LanguageCode.PT:
                    return new StemmerTokenFilter { Language = "portuguese" };
                case LanguageCode.DE:
                    return new StemmerTokenFilter { Language = "german" };
                default:
                    return new StemmerTokenFilter { Language = "english" };
            }
        }

        private StopTokenFilter GetStopwordFilter(LanguageCode languageCode)
        {
            return new StopTokenFilter
            {
                Stopwords = new string[] { "a", "an", "and", "are", "as", "at", "be", "but", "by", "for", "if", "into", "is", "no", "not", "of", "on", "such", "that", "the", "their", "then", "there", "these", "they", "this", "to", "was", "will", "with", "job", "jobs", "??", "??" }
            };
        }

        private WordDelimiterTokenFilter GetWordDelimeterTokenFilter(LanguageCode languageCode)
        {
            return new WordDelimiterTokenFilter
            {
                PreserveOriginal = true,
                CatenateAll = true,
                SplitOnNumerics = false
            };
        }

        private CustomAnalyzer GetJobDefaultAnanyzer(LanguageCode languageCode)
        {
            switch (languageCode)
            {
                case LanguageCode.ZH:
                    return new CustomAnalyzer()
                    {
                        Tokenizer = "whitespace",
                        Filter = new string[] { "cjk_width", "cjk_bigram", "lowercase", "asciifolding", "wdtFitler", "stopwordFilter", "snowballPorterFilter" }
                    };

                default:
                    return new CustomAnalyzer()
                    {
                        Tokenizer = "whitespace",
                        Filter = new string[] { "lowercase", "asciifolding", "wdtFitler", "stopwordFilter", "snowballPorterFilter" }
                    };
            }
        }

        private CustomAnalyzer GetJobDefaultWithDelimiterAnalyzer(LanguageCode languageCode)
        {
            switch (languageCode)
            {
                case LanguageCode.ZH:
                    return new CustomAnalyzer()
                    {
                        Tokenizer = "whitespace",
                        Filter = new string[] { "cjk_width", "cjk_bigram", "lowercase", "asciifolding", "wdtFitler", "stopwordFilter", "word_delimiter", "snowballPorterFilter" }
                    };

                default:
                    return new CustomAnalyzer()
                    {
                        Tokenizer = "whitespace",
                        Filter = new string[] { "lowercase", "asciifolding", "wdtFitler", "stopwordFilter", "word_delimiter", "snowballPorterFilter" }
                    };
            }
        }

        private CustomAnalyzer GetTitleSuggestAnalyzer(LanguageCode languageCode)
        {
            switch (languageCode)
            {
                case LanguageCode.ZH:
                    return new CustomAnalyzer
                    {
                        Tokenizer = "standard",
                        Filter = new string[] { "cjk_width", "lowercase", "cjk_bigram", "standard", "asciifolding" }
                    };

                default:
                    return new CustomAnalyzer
                    {
                        Tokenizer = "standard",
                        Filter = new string[] { "standard", "lowercase", "asciifolding" }
                    };
            }
        }
    }
}
