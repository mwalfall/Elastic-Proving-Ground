namespace LocationDulpicateNameAnalyzer.Model
{
    public class LocationHierarchyAnalysis
    {
        public long Id { get; set; }
        public long Population { get; set; }
        public string PName { get; set; }
        public string CountryCode { get; set; }
        public long MappedToId { get; set; }
    }
}
