using LocationDulpicateNameAnalyzer.DTO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace LocationDulpicateNameAnalyzer.Services
{
    public class XmlDataService
    {
        private string _destinationPath;
        private FileInfo _destinationFile;

        /// <summary>
        /// Given a country code obtain the corresponding xml document from disk
        /// and trnasform it into a list of ElasticsearchLocation objects.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="options">LocationIndexConfigurationOption object</param>
        /// 
        public IEnumerable<T> Get<T>(ConfigurationOptions options, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return new List<T>();

            SetLocationCountryFile(options, countryCode);

            var serializer = new XmlSerializer(typeof(List<T>));
            using (var fileStream = new FileStream(_destinationFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = XmlReader.Create(fileStream);
                var items = (List<T>)serializer.Deserialize(reader);
                return items;
            }
        }

        private void SetLocationCountryFile(ConfigurationOptions options, string countryCode)
        {
            _destinationFile = new FileInfo(options.XmlDocumentPath + "\\" + "Location" + countryCode + ".xml");
        }
    }
}
