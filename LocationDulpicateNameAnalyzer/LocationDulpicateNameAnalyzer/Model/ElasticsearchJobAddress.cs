using Nest;
using System;

namespace LocationDulpicateNameAnalyzer.Model
{
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
}
