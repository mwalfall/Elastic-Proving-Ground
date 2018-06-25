using System.Collections.Generic;

namespace LocationDulpicateNameAnalyzer.DTO
{
    public class ConfigurationOptions
    {
        public List<string> CountryCodes { get; set; }
        public string XmlDocumentPath { get; set; }
        public string LanguageCode { get; set; }
    }
}
