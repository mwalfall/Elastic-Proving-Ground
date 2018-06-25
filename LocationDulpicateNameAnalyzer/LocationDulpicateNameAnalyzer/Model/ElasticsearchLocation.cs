using Nest;
using System;
using System.Collections.Generic;

namespace LocationDulpicateNameAnalyzer.Model
{
    [Serializable]
    public class ElasticsearchLocation : ElasticsearchRootDocument
    {
        public ElasticsearchLocation()
        {
            this.Geometry = new ElasticsearchGeometry();
            this.AlternateNames = new List<string>();
        }

        [ElasticProperty(Name = "a")]
        public string HierarchyPath { get; set; }

        [ElasticProperty(Name = "b")]
        public int TypeID { get; set; }

        [ElasticProperty(Name = "c", Analyzer = "simple")]
        public string CountryCode { get; set; }

        [ElasticProperty(Name = "d")]
        public IEnumerable<string> CountryCtx { get; set; }

        [ElasticProperty(Name = "e")]
        public string Country { get; set; }

        [ElasticProperty(Name = "f", Analyzer = "simple")]
        public string Division1Code { get; set; }

        [ElasticProperty(Name = "g")]
        public string Division1 { get; set; }

        [ElasticProperty(Name = "h")]
        public string Division2 { get; set; }

        [ElasticProperty(Name = "i")]
        public string Division3 { get; set; }

        [ElasticProperty(Name = "j")]
        public string Division4 { get; set; }

        [ElasticProperty(Name = "k")]
        public string City { get; set; }

        [ElasticProperty(Name = "l")]
        public string FormattedName { get; set; }

        [ElasticProperty(Name = "m")]
        public long Population { get; set; }

        [ElasticProperty(Name = "n", Type = FieldType.GeoPoint)]
        public ElasticsearchGeometry Geometry { get; set; }

        [ElasticProperty(Name = "suggest")]
        public ElasticsearchLocationSuggestion Suggest { get; set; }

        [ElasticProperty(Name = "o")]
        public IEnumerable<string> AlternateNames { get; set; }
    }
}
