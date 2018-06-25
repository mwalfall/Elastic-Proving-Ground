namespace Domain.Model
{
    public class LocationAlias
    {
        public long SourceId { get; set; }
        public string Name { get; set; }
        public string LanguageCode { get; set; }
        public bool IsPreferredName { get; set; }
        public bool IsShortName { get; set; }
        public bool IsColloquial { get; set; }
        public bool IsHistoric { get; set; }
    }
}
