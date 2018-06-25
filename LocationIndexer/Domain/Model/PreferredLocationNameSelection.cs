namespace Domain.Model
{
    public class PreferredLocationNameSelection
    {
        public long SourceID { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public bool UseCustomName { get; set; }
        public bool UsePreferredName { get; set; }
        public bool UseShortName { get; set; }
    }
}
