using Nest;
using System;
using System.Collections.Generic;

namespace JobIndexBuilder.Domain
{
    [Serializable]
    public class ElasticsearchLocationV1 : ElasticsearchRootDocument
    {
        public ElasticsearchLocationV1()
        {
            this.Geometry = new ElasticsearchGeometry();
            this.AlternateNames = new List<string>();
        }

        [ElasticProperty(Name = "hierarchy_id")]
        public long HierarchyID { get; set; }

        [ElasticProperty(Name = "hierarchy_path")]
        public string HierarchyPath { get; set; }

        [ElasticProperty(Name = "type")]
        public string Type { get; set; }

        [ElasticProperty(Name = "type_id")]
        public int TypeID { get; set; }

        [ElasticProperty(Name = "code")]
        public string Code { get; set; }

        [ElasticProperty(Name = "country_code", Analyzer = "simple")]
        public string CountryCode { get; set; }

        [ElasticProperty(Name = "countryctx")]
        public List<string> CountryCtx { get; set; }

        [ElasticProperty(Name = "country")]
        public string Country { get; set; }

        [ElasticProperty(Name = "division_1_code", Analyzer = "simple")]
        public string Division1Code { get; set; }

        [ElasticProperty(Name = "division_1")]
        public string Division1 { get; set; }

        [ElasticProperty(Name = "division_2_code", Analyzer = "simple")]
        public string Division2Code { get; set; }

        [ElasticProperty(Name = "division_2")]
        public string Division2 { get; set; }

        [ElasticProperty(Name = "division_3_code", Analyzer = "simple")]
        public string Division3Code { get; set; }

        [ElasticProperty(Name = "division_3")]
        public string Division3 { get; set; }

        [ElasticProperty(Name = "division_4_code", Analyzer = "simple")]
        public string Division4Code { get; set; }

        [ElasticProperty(Name = "division_4")]
        public string Division4 { get; set; }

        [ElasticProperty(Name = "city")]
        public string City { get; set; }

        [ElasticProperty(Name = "formatted_name")]
        public string FormattedName { get; set; }

        [ElasticProperty(Name = "population")]
        public long Population { get; set; }

        [ElasticProperty(Name = "geometry", Type = FieldType.GeoPoint)]
        public ElasticsearchGeometry Geometry { get; set; }

        [ElasticProperty(Name = "suggest")]
        public ElasticsearchLocationSuggestion Suggest { get; set; }

        [ElasticProperty(Name = "alternate_names")]
        public List<string> AlternateNames { get; set; }

        [ElasticProperty(Name = "job_addresses")]
        public List<ElasticsearchJobAddress> JobAddresses { get; set; }
    }
}
