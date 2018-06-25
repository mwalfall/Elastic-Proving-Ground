namespace Domain.Model
{
    public class LocationName
    {
        public long ID { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public long SourceID { get; set; }
        public string Name { get; set; }
    }
}
