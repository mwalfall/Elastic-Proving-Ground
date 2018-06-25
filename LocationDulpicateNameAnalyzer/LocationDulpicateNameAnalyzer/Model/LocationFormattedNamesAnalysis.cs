namespace LocationDulpicateNameAnalyzer.Model
{
    public class LocationFormattedNamesAnalysis
    {
        public long ID { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public string FormattedName { get; set; }
        public string FormattedNameNon { get; set; }
        public int TypeId { get; set; }
        public int Formatting { get; set; }
    }
}
