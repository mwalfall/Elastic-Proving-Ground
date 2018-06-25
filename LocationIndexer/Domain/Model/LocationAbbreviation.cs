using System;
using System.Collections.Generic;
using System.Linq;
namespace Domain.Model
{
    public class LocationAbbreviation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegexString { get; set; }
        public string Abbreviations { get; set; }
    }
}
