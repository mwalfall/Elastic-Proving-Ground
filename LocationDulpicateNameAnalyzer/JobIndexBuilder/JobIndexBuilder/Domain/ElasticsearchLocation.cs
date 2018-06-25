using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobIndexBuilder.Domain
{
    public class ElasticsearchLocation : ElasticsearchRootDocument
    {
        public ElasticsearchLocation()
        {
            this.Geometry = new ElasticsearchGeometry();
            this.AlternateNames = new List<string>();
        }

        [ElasticProperty(Name = "hierarchy_id")]
        public long HierarchyID { get; set; }

        [ElasticProperty(Name = "a")]
        public string HierarchyPath { get; set; }

        [ElasticProperty(OptOut = true)]
        public string Type { get; set; }

        [ElasticProperty(Name = "b")]
        public int TypeID { get; set; }

        [ElasticProperty(OptOut = true)]
        public string Code { get; set; }

        [ElasticProperty(Name = "c", Analyzer = "simple")]
        public string CountryCode { get; set; }

        [ElasticProperty(Name = "d")]
        public List<string> CountryCtx { get; set; }

        [ElasticProperty(Name = "e")]
        public string Country { get; set; }

        [ElasticProperty(Name = "f", Analyzer = "simple")]
        public string Division1Code { get; set; }

        [ElasticProperty(Name = "g")]
        public string Division1 { get; set; }

        [ElasticProperty(OptOut = true)]
        public string Division2Code { get; set; }

        [ElasticProperty(Name = "h")]
        public string Division2 { get; set; }

        [ElasticProperty(OptOut = true)]
        public string Division3Code { get; set; }

        [ElasticProperty(Name = "i")]
        public string Division3 { get; set; }

        [ElasticProperty(OptOut = true)]
        public string Division4Code { get; set; }

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
        public List<string> AlternateNames { get; set; }

        [ElasticProperty(Name = "job_addresses")]
        public List<ElasticsearchJobAddress> JobAddresses { get; set; }
    }

    [Serializable]
    public class ElasticsearchJobAddress : ElasticsearchRootDocument
    {
        [ElasticProperty(Name = "organization_address_id")]
        public int OrganizationAddressID { get; set; }

        [ElasticProperty(Name = "office_name")]
        public string OfficeName { get; set; }

        [ElasticProperty(Name = "addressline1")]
        public string AddressLine1 { get; set; }

        [ElasticProperty(Name = "addressline2")]
        public string AddressLine2 { get; set; }

        [ElasticProperty(Name = "zip")]
        public string Zip { get; set; }

        [ElasticProperty(Name = "isDefault")]
        public bool IsDefault { get; set; }

        [ElasticProperty(Name = "active")]
        public bool Active { get; set; }

        [ElasticProperty(Name = "locationid")]
        public long? LocationID { get; set; }
    }

    [Serializable]
    public class ElasticsearchGeometry
    {
        [ElasticProperty(Name = "lat")]
        public float Latitude { get; set; }

        [ElasticProperty(Name = "lon")]
        public float Longitude { get; set; }
    }

    [Serializable]
    public class ElasticsearchLocationSuggestion
    {
        [ElasticProperty(Name = "input")]
        public List<string> Input { get; set; }

        [ElasticProperty(Name = "output")]
        public string Output { get; set; }

        [ElasticProperty(Name = "payload")]
        public ElasticsearchLocationPayload Payload { get; set; }

        [ElasticProperty(Name = "weight")]
        public uint? Weight { get; set; }
    }

    [Serializable]
    public class ElasticsearchLocationPayload
    {
        [ElasticProperty(Name = "id")]
        public long ID { get; set; }

        [ElasticProperty(Name = "hierarchy_path")]
        public string HierarchyPath { get; set; }

        [ElasticProperty(Name = "country")]
        public string Country { get; set; }

        [ElasticProperty(Name = "country_code")]
        public string CountryCode { get; set; }

        [ElasticProperty(Name = "division_1")]
        public string Division1 { get; set; }

        [ElasticProperty(Name = "division_1_code")]
        public string Division1Code { get; set; }

        [ElasticProperty(Name = "division_2")]
        public string Division2 { get; set; }

        [ElasticProperty(Name = "division_3")]
        public string Division3 { get; set; }

        [ElasticProperty(Name = "division_4")]
        public string Division4 { get; set; }

        [ElasticProperty(Name = "city")]
        public string City { get; set; }

        [ElasticProperty(Name = "type_id")]
        public int TypeID { get; set; }

        [ElasticProperty(Name = "lat")]
        public float Latitude { get; set; }

        [ElasticProperty(Name = "lon")]
        public float Longitude { get; set; }
    }
}
