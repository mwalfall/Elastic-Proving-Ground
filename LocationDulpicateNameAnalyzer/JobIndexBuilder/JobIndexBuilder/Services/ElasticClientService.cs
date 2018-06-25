using Elasticsearch.Net.ConnectionPool;
using JobIndexBuilder.Domain;
using JobIndexBuilder.DTO;
using JobIndexBuilder.Elasticsearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobIndexBuilder.Services
{
    public class ElasticClientService
    {
        private ElasticClient _elasticClient = null;
        private IConnectionSettingsValues _connectionSettings;
        private IElasticsearchAnalyzerResolver _analyzerResolver = new ElasticsearchAnalyzerResolver();
        private readonly IElasticsearchAnalysisResolver _analysisResolver = new ElasticsearchAnalysisResolver();
        private readonly IElasticsearchMappingResolver _mappingResolver = new ElasticsearchMappingResolver();
        private readonly EnvironmentContext _envrionmentContext;

        public ElasticClientService(EnvironmentContext environmentContext)
        {
            _envrionmentContext = environmentContext;
            CreateClient();
        }

        #region Public Methods

        public ElasticClient GetClient()
        {

            if (_elasticClient != null)
                return _elasticClient;

            CreateClient();
            return _elasticClient;
        }

        /// <summary>
        /// Get list of indices pointing to an alias
        /// </summary>
        /// <param name="alias">Alias name</param>
        ///
        public IEnumerable<string> GetIndicesPointingToAlias(string alias)
        {
            return _elasticClient.GetIndicesPointingToAlias(alias);
        }

        /// <summary>
        /// Remove an alias associated with an index.
        /// </summary>
        /// <param name="index">The name of the index.</param>
        /// <param name="alias">The name of the alias to remove.</param>
        public void RemoveAlias(string index, string indexAlias, string groupAlias)
        {
            _elasticClient.Alias(s => s.Remove(a => a.Index(index).Alias(indexAlias)));
            _elasticClient.Alias(s => s.Remove(a => a.Index(index).Alias(groupAlias)));
        }

        /// <summary>
        /// Set alias in on atomic operation.
        /// </summary>
        /// <param name="aliasName">Alias Name</param>
        /// <param name="oldIndexName">Old Index Name</param>
        /// <param name="newIndexName">New Index Name</param>
        /// 
        public void SetAlias(string aliasName, string oldIndexName, string newIndexName)
        {
            _elasticClient.Alias(a => a
                .Add(add => add
                    .Index(newIndexName)
                    .Alias(aliasName)
                )
                .Remove(remove => remove
                    .Index(oldIndexName)
                    .Alias(aliasName)
                )
            );
        }

        /// <summary>
        /// Add Alias
        /// </summary>
        /// <param name="index">Index name</param>
        /// <param name="alias">Alias name</param>
        /// 
        public void AddAlias(string index, string alias)
        {
            _elasticClient.Alias(s => s.Add(a => a.Index(index).Alias(alias)));
        }

        /// <summary>
        /// Create index
        /// </summary>
        /// <typeparam name="T">Index type</typeparam>
        /// <param name="index">Index name</param>
        /// <param name="numberOfShards">Number of shards</param>
        /// <param name="numberOfReplicas">Number of replicas</param>
        /// <param name="force">Force</param>
        /// <param name="languageCode">Language Code Enum</param>
        /// 
        public bool CreateIndex<T>(string index, int numberOfShards, int numberOfReplicas, bool force = false, LanguageCode languageCode = LanguageCode.EN)
            where T : class
        {
            var descriptor = new CreateIndexDescriptor(_connectionSettings);
            descriptor.NumberOfShards(numberOfShards);
            descriptor.NumberOfReplicas(numberOfReplicas);

            var analysis = _analysisResolver.Resolve<T>(languageCode);

            if (analysis != null)
                descriptor.Analysis(a => analysis);

            // Add any custom analyzers
            _analyzerResolver = new ElasticsearchAnalyzerResolver();
            if (_analysisResolver != null)
                descriptor = _analyzerResolver.Resolve<T>(descriptor);

            var mapping = _mappingResolver.Resolve<T>(_connectionSettings);

            if (mapping != null)
                descriptor.AddMapping<T>(m => mapping);

            var success = CreateIndex(index, descriptor, force);

            return success;
        }

        /// <summary>
        /// Create index
        /// </summary>
        /// <param name="index">Index name</param>
        /// <param name="descriptor">CreateIndexDescriptor object</param>
        /// <param name="force">Force</param>
        /// 
        private bool CreateIndex(string index, CreateIndexDescriptor descriptor, bool force)
        {
            if (string.IsNullOrWhiteSpace(index))
                throw new ArgumentNullException("index", "An index name was not provided.");

            if (descriptor == null)
                throw new ArgumentNullException("descriptor", "An object of type CreateIndexDescriptor was not provided.");

            var exists = IndexExists(index);

            if (!exists || force)
            {
                if (exists)
                    _elasticClient.DeleteIndex(d => d.Index(index));

                var response = _elasticClient.CreateIndex(index, d => descriptor);

                return response.IsValid;
            }

            return false;
        }

        /// <summary>
        /// Determines if index exists
        /// </summary>
        /// <param name="index">Index name</param>
        /// 
        public bool IndexExists(string index)
        {
            var response = _elasticClient.IndexExists(s => s.Index(index));
            var exists = (response.IsValid && response.Exists);

            return exists;
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Instantiate EsClient.
        /// </summary>
        private void CreateClient()
        {
            var values = GetConnectionSettings(_envrionmentContext.Uris.Split(',').Select(uri => new Uri(uri)));
            _connectionSettings = values;
            _elasticClient = new ElasticClient(values);
        }

        /// <summary>
        /// Get connection string settings for ES.
        /// </summary>
        /// <param name="uris">List of URIs</param>
        /// 
        private IConnectionSettingsValues GetConnectionSettings(IEnumerable<Uri> uris)
        {
            var settings = new ConnectionSettings(new SniffingConnectionPool(uris), "default")
                .ExposeRawResponse(false)
                .SetTimeout(600000); // 10 Minutes

            return settings;
        }
        #endregion Private Methods
    }
}
