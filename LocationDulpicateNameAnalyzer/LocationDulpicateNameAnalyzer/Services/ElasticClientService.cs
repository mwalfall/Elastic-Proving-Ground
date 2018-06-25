using Elasticsearch.Net.ConnectionPool;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LocationDulpicateNameAnalyzer.Services
{
    public class ElasticClientService
    {
        private ElasticClient _elasticClient = null;

        public ElasticClientService()
        {
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
        #endregion Public Methods

        #region Private Methods
        private void CreateClient()
        {
            var uris = ConfigurationManager.ConnectionStrings["Search"].ConnectionString;
            var values = GetConnectionSettings(uris.Split(',').Select(uri => new Uri(uri)));
            _elasticClient = new ElasticClient(values);
        }
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
