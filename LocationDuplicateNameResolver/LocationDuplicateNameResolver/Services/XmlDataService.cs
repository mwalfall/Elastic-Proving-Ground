using LocationDuplicateNameResolver.DTO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace LocationDuplicateNameResolver.Services
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
        public IEnumerable<T> Get<T>(ConfigurationOptions options)
        {
            if (!options.CountryCodes.Any())
                return new List<T>();

            SetLocationCountryFile(options);

            var serializer = new XmlSerializer(typeof(List<T>));
            using (var fileStream = new FileStream(_destinationFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = XmlReader.Create(fileStream);
                var items = (List<T>)serializer.Deserialize(reader);
                return items;
            }
        }

        private void SetLocationCountryFile(ConfigurationOptions options)
        {
            _destinationFile = new FileInfo(options.XmlDocumentPath + "\\" + "Location" + options.CountryCodes.First() + ".xml");
        }
    }
}
