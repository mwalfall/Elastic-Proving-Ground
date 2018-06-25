using LocationIndexer.LocationBuilders;
using System.Collections.Generic;
namespace LocationIndexer.Services.Interfaces
{
    public interface IXmlDataService
    {
        /// <summary>
        /// A generic method that acts as a controller of the process to
        /// convert the provided list to an xml document that is saved to disk.
        /// </summary>
        /// <typeparam name="T">The type of the List provided.</typeparam>
        /// <param name="sourceList">List that is to be stored as xml document</param>
        /// <param name="fileName">File name for the xml document</param>
        /// <param name="destinationPath">Working folder where xml documents are to be stored.</param>
        /// 
        void SetToXml<T>(IList<T> sourceList, string fileName, string destinationPath);

        /// <summary>
        /// Given a country code obtain the corresponding xml document from disk
        /// and trnasform it into a list of ElasticsearchLocation objects.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="options">GlobalContext object</param>
        /// 
        IEnumerable<T> Get<T>(string countryCode, GlobalContext options);
    }
}
