using JobIndexBuilder.Domain;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobIndexBuilder.Elasticsearch
{
    public interface IElasticsearchAnalysisResolver
    {
        /// <summary>
        /// Determines the analyzer that is to be used for different types.
        /// </summary>
        /// <typeparam name="T">Elasticsearch type</typeparam>
        /// <param name="languageCode">LanguageCode enum value</param>
        /// 
        AnalysisDescriptor Resolve<T>(LanguageCode languageCode = LanguageCode.EN);
    }
}
