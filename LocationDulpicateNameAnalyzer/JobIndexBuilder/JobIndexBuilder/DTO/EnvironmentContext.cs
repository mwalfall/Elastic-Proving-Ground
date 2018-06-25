using JobIndexBuilder.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace JobIndexBuilder.DTO
{
    public class EnvironmentContext
    {

        private string _uris;
        private string _environment;
        private bool _isIndexByAlias;
        private string _indexAliasValues;
        private string _indexNameValues;
        private List<string> _indexAliasList;
        private List<string> _indexNameList;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandLineOptions">CommandLineOptions object</param>
        /// 
        public EnvironmentContext(CommandLineOptions commandLineOptions)
        {
            _environment = commandLineOptions.GetOptionValue<string>("environment").ToLower();
            var indexby = commandLineOptions.GetOptionValue<string>("indexby").ToLower().Trim();
            _indexNameValues = commandLineOptions.GetOptionValue<string>("indexnamevalues").ToLower();
            _indexAliasValues = commandLineOptions.GetOptionValue<string>("indexaliasvalues").ToLower();

            switch(indexby)
            {
                case "name":
                    _isIndexByAlias = false;
                    SetIndexNameValues();
                    break;

                case "alias":
                    _isIndexByAlias = true;
                    SetIndexAliasValues();
                    break;

                default:
                    _isIndexByAlias = true;
                    SetIndexAliasValues();
                    break;
            }

            SetElasticsearchConnString();
        }
        #endregion Constructor

        #region Public Properties

        public bool IsIndexByAlias { get { return _isIndexByAlias; } }
        public string Uris { get { return _uris; } }
        public List<string> IndexAliasValues { get { return _indexAliasList; } }
        public List<string> IndexNameValues { get { return _indexNameList; } }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Get List of country codes.
        /// Assumes  string is in valid format.
        /// </summary>
        /// <param name="countriesString">string containing country codes</param>
        /// 
        private void SetIndexAliasValues()
        {
            if (string.IsNullOrWhiteSpace(_indexAliasValues))
            {
                SetDefaultAliasNames();
                return;
            }

            var values = _indexAliasValues;
            var location = values.IndexOf('[');
            values = values.Remove(location, 1);
            location = values.IndexOf(']');
            values = values.Remove(location, 1);
            _indexAliasList = values.Split(',').ToList();          
        }

        private void SetIndexNameValues()
        {
            var values = _indexNameValues;
            var location = values.IndexOf('[');
            values = values.Remove(location, 1);
            location = values.IndexOf(']');
            values = values.Remove(location, 1);
            _indexNameList = values.Split(',').ToList();
        }

        /// <summary>
        /// Set the connection string for the Elasticsearch server.
        /// </summary>
        /// <param name="environment">Environment to process. Values: local, dev, qa, stg, prod.</param>
        private void SetElasticsearchConnString()
        {
            var indexEnvironment = "index-" + _environment.ToLower().Trim();
            _uris = ConfigurationManager.ConnectionStrings[indexEnvironment].ConnectionString;
        }

        /// <summary>
        /// Set the default alias names.
        /// </summary>
        private void SetDefaultAliasNames()
        {
            var appSettingsKey = "alias-names";
            _indexAliasList = ConfigurationManager.AppSettings[appSettingsKey].ToString().Trim().Split(',').ToList();
        }
        #endregion Private Methods
    }
}
