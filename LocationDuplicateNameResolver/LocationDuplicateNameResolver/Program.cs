using LocationDuplicateNameResolver.DTO;
using LocationDuplicateNameResolver.Model;
using LocationDuplicateNameResolver.Services;
using System;
using System.Collections.Generic;

namespace LocationDuplicateNameResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ConfigurationOptions
            {
                CountryCodes = new List<string> { "GB" },
                XmlDocumentPath = @"\\ServerName\GeoNamesData\LocationXml"
            };

            var xmlDataService = new XmlDataService();
            var documents = xmlDataService.Get<LocationFormattedName>(options);

            Console.WriteLine("**** Procesing completed successfully. ****");
            Environment.Exit(0);
        }
    }
}
