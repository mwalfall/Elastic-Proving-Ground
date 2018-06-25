namespace Domain.Model
{
    public class LocationSuggestionInput
    {
        public int Id { get; set; }
        public long SourceId { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }
    }
}
