using LocationIndexer.LocationBuilders;
using LocationIndexer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LocationIndexer.Services
{
    public class XmlDataService : IXmlDataService
    {
        private string _destinationPath;
        private FileInfo _destinationFile;

        #region Public Methods

        /// <summary>
        /// A generic method that acts as a controller of the process to
        /// convert the provided list to an xml document that is saved to disk.
        /// </summary>
        /// <typeparam name="T">The type of the List provided.</typeparam>
        /// <param name="sourceList">List that is to be stored as xml document</param>
        /// <param name="fileName">File name for the xml document</param>
        /// <param name="destinationPath">Working folder where xml documents are to be stored.</param>
        /// 
        public void SetToXml<T>(IList<T> sourceList, string fileName, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("A destination path is required.");

            _destinationPath = destinationPath;
            SetDestinationFile(fileName);
            SerializeData(sourceList);
        }

        /// <summary>
        /// Given a country code obtain the corresponding xml document from disk
        /// and trnasform it into a list of ElasticsearchLocation objects.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="options">GlobalContext object</param>
        /// 
        public IEnumerable<T> Get<T>(string countryCode, GlobalContext options)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return new List<T>();

            SetLocationCountryFile(countryCode, options);

            var serializer = new XmlSerializer(typeof(List<T>));
            using (var fileStream = new FileStream(_destinationFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = XmlReader.Create(fileStream);
                var items = (List<T>)serializer.Deserialize(reader);
                return items;
            }
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Determines if the file is presently on the disk. If so, it is deleted.
        /// </summary>
        /// <param name="destinationFile">Destination File</param>
        /// 
        private static void PrepareFileOnDisk(FileInfo destinationFile)
        {
            if (destinationFile.Exists)
                destinationFile.Delete();
        }

        /// <summary>
        /// Converts the provided list to an xml document.
        /// </summary>
        /// <typeparam name="T">The type of the List</typeparam>
        /// <param name="sourceList">List that is to be stored as xml document</param>
        /// 
        private void SerializeData<T>(IList<T> sourceList)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            Stream fs = new FileStream(_destinationFile.FullName, FileMode.Create);

            using (var writer = new XmlTextWriter(fs, Encoding.Unicode))
            {
                serializer.Serialize(writer, sourceList);
            }
        }

        /// <summary>
        /// Generic version for setting file names
        /// </summary>
        /// <param name="fileName">string containing file name</param>
        /// 
        private void SetDestinationFile(string fileName)
        {
            _destinationFile = new FileInfo(_destinationPath + "\\" + fileName);
            PrepareFileOnDisk(_destinationFile);
        }

        /// <summary>
        /// Set the filename and the path to that file.
        /// </summary>
        /// <param name="countryCode">CountryCode</param>
        /// <param name="options">GlobalContext object</param>
        private void SetLocationCountryFile(string countryCode, GlobalContext options)
        {
            _destinationFile = new FileInfo(options.EnvironmentContext.XmlDocumentsDestinationPath + "\\" + "Location" + countryCode + ".xml");
        }
        #endregion Private Methods
    }
}
