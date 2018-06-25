using System;
using System.Collections.Generic;
namespace Domain.Model
{
    public class LocationCleanserDirective
    {
        public long Id { get; set; }
        public string CountryCode { get; set; }
        public long SourceId { get; set; }
        public int Action { get; set; }
    }
}
