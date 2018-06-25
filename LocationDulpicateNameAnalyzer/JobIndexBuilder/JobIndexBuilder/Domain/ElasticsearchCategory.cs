using Nest;
using System;
using System.Collections.Generic;

namespace JobIndexBuilder.Domain
{
    [Serializable]
    public class ElasticsearchCategory : ElasticsearchRootDocument
    {
        [ElasticProperty(Name = "tenant_id")]
        public long TenantID { get; set; }

        [ElasticProperty(Name = "name")]
        public string Name { get; set; }

        [ElasticProperty(Name = "description")]
        public string Description { get; set; }

        [ElasticProperty(Name = "language_id")]
        public int LanguageID { get; set; }

        [ElasticProperty(Name = "hidden")]
        public bool Hidden { get; set; }

        [ElasticProperty(Name = "group_id")]
        public Nullable<long> GroupID { get; set; }

        [ElasticProperty(Name = "categorygroup")]
        public bool IsCategoryGroup { get; set; }

        [ElasticProperty(Name = "division_copy")]
        public List<ElasticsearchCategoryDescription> Descriptions { get; set; }
    }

    [Serializable]
    public class ElasticsearchCategoryDescription
    {
        [ElasticProperty(Name = "organization_id")]
        public int OrganizationID { get; set; }

        [ElasticProperty(Name = "description")]
        public string Description { get; set; }
    }
}
