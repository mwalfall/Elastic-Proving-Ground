using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocationDuplicateNameResolver.Model
{
    public class LocationFormattedName
    {
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [Key, Column(Order = 1)]
        public string CountryCode { get; set; }
        [Key, Column(Order = 2)]
        public string IndexLanguage { get; set; }
        public string Name { get; set; }
    }
}

