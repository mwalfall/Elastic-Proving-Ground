using LocationIndexer.LocationBuilders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LocationIndexer
{
    public class EnvironmentContext
    {
        #region Private Instance Variables

        private bool _isIndexSelectedCountries;
        private string _indexPrefix;
        private string _indexLanguage;
        private string _environment;
        private string _indexName;
        private string _xmlDocumentsDestinationPath;
        private Build _build;
        private List<string> _countryGroups;

        #endregion Private Instance Variables

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="environment">Environment context. Default is 'local'.</param>
        /// 
        public EnvironmentContext(RuntimeParameters runtimeParameters)
        {
            SetEnvironmentContext(runtimeParameters);
        }
        #endregion Contructor

        #region Public Properties

        public Build Build { get { return _build; } }
        public string Environment { get { return _environment; } }
        public string IndexLanguage { get { return _indexLanguage; } }
        public string IndexName {  get { return _indexName; } }
        public bool IsIndexSelectedCountries { get { return _isIndexSelectedCountries; } }      
        public List<string> LanguageCodes { get; set; }
        public ConnectionStringSettings PlatformConnection { get; set; }
        public string XmlDocumentsDestinationPath { get { return _xmlDocumentsDestinationPath; } }
        public List<string> CountryGroups { get { return _countryGroups; } }
        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Setup the operating environment
        /// </summary>
        /// <param name="environment">Environment to process within.</param>
        private void SetEnvironmentContext(RuntimeParameters runtimeParameters)
        {
            _environment = !string.IsNullOrWhiteSpace(runtimeParameters.Environment) ? runtimeParameters.Environment : "local";
            _indexLanguage = !string.IsNullOrWhiteSpace(runtimeParameters.IndexLanguage) ? runtimeParameters.IndexLanguage : "en";
            _build = runtimeParameters.Build;

            SetSqlEnvironments();
            SetCountriesToBeIndexed();
            SetLanguageCodes();
            SetIndexName();
            SetXmlDocumentsDestinationPath();

            //SetElasticsearchConnString(environment);
            //SetShardsReplicasValues(environment);
            //SetDocumentPartitionTargetSizeMultiplier(environment);
        }

        /// <summary>
        /// Set countries to be indexed.
        /// </summary>
        private void SetCountriesToBeIndexed()
        {
            _isIndexSelectedCountries = ConfigurationManager.AppSettings["IsIndexSelectedCountries"].ToLower().Equals("true");
            if (_isIndexSelectedCountries)
            {
                _countryGroups = ConfigurationManager.AppSettings["CountriesToIndex"].Split(',').ToList();
            } 
        }

        /// <summary>
        /// Folder where XML files are to stored.
        /// </summary>
        private void SetXmlDocumentsDestinationPath()
        {
            var settingKey = "XmlDocumentsDestinationPath-" + Environment;
            _xmlDocumentsDestinationPath = ConfigurationManager.AppSettings[settingKey] + IndexLanguage.ToLower();
        }

        /// <summary>
        /// Get the prefix for the index.
        /// </summary>
        private void SetIndexName()
        {
            var result = ConfigurationManager.AppSettings["IndexPrefix"];
            _indexPrefix = (!string.IsNullOrWhiteSpace(result)) ? result : "locations";
            _indexName = _indexPrefix + "-" + _indexLanguage.Trim() + "-" + DateTime.Now.ToString("MM.dd.yy.HH.mm.ss");
        }

        /// <summary>
        /// Ste the languages that are to be processed.
        /// </summary>
        /// <param name="environmentContext">EnvironmentContext object</param>
        /// 
        private void SetLanguageCodes()
        {
            var appSettingsKey = "LanguageCodes";
            var languageCodes = ConfigurationManager.AppSettings[appSettingsKey].ToString().Trim().Split(',');

            LanguageCodes = languageCodes.ToList();
        }

        /// <summary>
        /// Set the Platform databse connection string.
        /// </summary>
        /// <param name="environmentContext">EnvironmentContext object containing the environment to process. Values: local, dev, qa, stg, prod.</param>
        /// 
        private void SetSqlEnvironments()
        {
            var sqlEnviroment = "PlatformConnectionString-" + Environment.ToLower().Trim();
            PlatformConnection = ConfigurationManager.ConnectionStrings[sqlEnviroment];
        }
        #endregion Private Methods
    }
}
