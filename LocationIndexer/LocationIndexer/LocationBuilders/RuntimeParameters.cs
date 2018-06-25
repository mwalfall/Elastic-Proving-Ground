using System.Collections.Generic;

namespace LocationIndexer.LocationBuilders
{
    public class RuntimeParameters
    {
        #region Private Instance Variables

        private static List<string> Environments = new List<string> { "local", "dev", "qa", "stg", "prod" };
        private string _environment;
        private string _indexLanguage;
        private Build _build;
        private string _buildStr;

        #endregion Private Instance Variables

        #region Constructor

        public RuntimeParameters(string[] args)
        {
            if (args.Length == 2 &&
                (args[0].ToLower().ToLower().Equals("-environment") && Environments.Contains(args[1].ToLower().Trim())))
                _environment = args[1].ToLower().Trim();


            _indexLanguage = "en";
            _buildStr = "xml";
            _build = Build.Xml;
        }
        #endregion Constructor

        #region Properties

        public string Environment { get { return _environment; } }
        public string IndexLanguage { get { return _indexLanguage; } }
        public Build Build {  get { return _build; } }

        #endregion Properties
    }
}
