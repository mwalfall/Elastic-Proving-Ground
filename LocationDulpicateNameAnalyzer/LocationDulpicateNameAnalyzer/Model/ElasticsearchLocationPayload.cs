using Nest;
using System;

namespace LocationDulpicateNameAnalyzer.Model
{
    [Serializable]
    public class ElasticsearchLocationPayload
    {
        [ElasticProperty(Name = "id")]
        public long ID { get; set; }

        [ElasticProperty(Name = "a")]
        public string HierarchyPath { get; set; }

        [ElasticProperty(Name = "e")]
        public string Country { get; set; }

        [ElasticProperty(Name = "c")]
        public string CountryCode { get; set; }

        [ElasticProperty(Name = "g")]
        public string Division1 { get; set; }

        [ElasticProperty(Name = "f")]
        public string Division1Code { get; set; }

        [ElasticProperty(Name = "h")]
        public string Division2 { get; set; }

        [ElasticProperty(Name = "i")]
        public string Division3 { get; set; }

        [ElasticProperty(Name = "j")]
        public string Division4 { get; set; }

        [ElasticProperty(Name = "k")]
        public string City { get; set; }

        [ElasticProperty(Name = "b")]
        public int TypeID { get; set; }

        [ElasticProperty(Name = "q")]
        public float Latitude { get; set; }

        [ElasticProperty(Name = "r")]
        public float Longitude { get; set; }
    }
}
