using JobIndexBuilder.Elasticsearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobIndexBuilder.Domain
{
    [ElasticType(Name ="job")]
    public class ElasticsearchJob : ElasticsearchRootDocument
    {
        public ElasticsearchJob()
        {
            CustomFields = new Dictionary<string, object>();
        }

        [ElasticProperty(Name = "office_name")]
        public string OfficeName { get; set; }

        [ElasticProperty(Name = "address1")]
        public string Address { get; set; }

        [ElasticProperty(Name = "address2")]
        public string Address2 { get; set; }

        [ElasticProperty(Name = "apply_url")]
        public String ApplyUrl { get; set; }

        [ElasticProperty(Name = "apply_urls")]
        public Dictionary<string, string> ApplyUrls { get; set; }

        [Facetable("Campaign", JobFieldType.String)]
        [ElasticProperty(Name = "campaign")]
        public string Campaign { get; set; }

        [ElasticProperty(Name = "categories")]
        public List<ElasticsearchJobCategory> Categories { get; set; }

        [ElasticProperty(Name = "category_locations")]
        public List<ElasticsearchCategoryLocation> CategoryLocations { get; set; }

        [ElasticProperty(Name = "brands")]
        public List<ElasticsearchJobBrand> Brands { get; set; }

        [ElasticProperty(Name = "city")]
        public string City { get; set; }

        [Facetable("Company Name", JobFieldType.String)]
        [ElasticProperty(Name = "company_name")]
        public string CompanyName { get; set; }

        [ElasticProperty(Name = "country")]
        public string Country { get; set; }

        [ElasticProperty(Name = "custom_fields")]
        public IDictionary<string, object> CustomFields { get; set; }

        [ElasticProperty(Name = "description")]
        public string Description { get; set; }

        [ElasticProperty(Name = "description_html")]
        public string DescriptionHtml { get; set; }

        [ElasticProperty(Name = "education")]
        public string Education { get; set; }

        [ElasticProperty(Name = "external_reference_code")]
        public string ExternalReferenceCode { get; set; }

        [ElasticProperty(Name = "feed_id")]
        public int FeedID { get; set; }

        [ElasticProperty(Name = "field1")]
        public string Field1 { get; set; }

        [ElasticProperty(Name = "field2")]
        public string Field2 { get; set; }

        [Facetable("Hours Per Week", JobFieldType.String)]
        [ElasticProperty(Name = "hours_per_week")]
        public string HoursPerWeek { get; set; }

        [ElasticProperty(Name = "import_id")]
        public string ImportID { get; set; }

        [Facetable("Industry", JobFieldType.String)]
        [ElasticProperty(Name = "industry")]
        public string Industry { get; set; }

        [ElasticProperty(Name = "inserted_date")]
        public DateTime? InsertedDate { get; set; }

        [ElasticProperty(Name = "posted_date")]
        public DateTime PostedDate { get; set; }

        [ElasticProperty(Name = "intro")]
        public string Intro { get; set; }

        [Facetable("Is Manager", JobFieldType.String)]
        [ElasticProperty(Name = "is_manager")]
        public string IsManager { get; set; }

        [Facetable("Is Telecommute", JobFieldType.String)]
        [ElasticProperty(Name = "is_telecommute")]
        public string IsTelecommute { get; set; }

        [Facetable("Job Level", JobFieldType.String)]
        [ElasticProperty(Name = "job_level")]
        public string JobLevel { get; set; }

        [Facetable("Job Status", JobFieldType.String)]
        [ElasticProperty(Name = "job_status")]
        public string JobStatus { get; set; }

        [Facetable("Job Type", JobFieldType.String)]
        [ElasticProperty(Name = "job_type")]
        public string JobType { get; set; }

        [ElasticProperty(Name = "language_code")]
        public string LanguageCode { get; set; }

        [ElasticProperty(Name = "language_id")]
        public int LanguageID { get; set; }

        [ElasticProperty(Name = "locations")]
        public List<ElasticsearchLocation> Locations { get; set; }

        [ElasticProperty(Name = "max_experience")]
        public string MaxExperience { get; set; }

        [ElasticProperty(Name = "min_experience")]
        public string MinExperience { get; set; }

        [ElasticProperty(Name = "organization_id")]
        public long OrganizationID { get; set; }

        [ElasticProperty(Name = "qualifications")]
        public string Qualifications { get; set; }

        [Facetable("Salary Relocation", JobFieldType.String)]
        [ElasticProperty(Name = "salary_relocation")]
        public string SalaryRelocation { get; set; }

        [Facetable("Salary Time", JobFieldType.String)]
        [ElasticProperty(Name = "salary_time")]
        public string SalaryTime { get; set; }

        [ElasticProperty(Name = "region")]
        public string State { get; set; }

        [ElasticProperty(Name = "tenant_id")]
        public long TenantID { get; set; }

        [ElasticProperty(Name = "title")]
        public string Title { get; set; }

        [Facetable("Travel", JobFieldType.String)]
        [ElasticProperty(Name = "travel")]
        public string Travel { get; set; }

        [ElasticProperty(Name = "updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [ElasticProperty(Name = "version")]
        public int Version { get; set; }

        [ElasticProperty(Name = "postal_code")]
        public string Zip { get; set; }

        [ElasticProperty(Name = "contact_name")]
        public string ContactName { get; set; }

        [ElasticProperty(Name = "contact_address")]
        public string ContactAddress { get; set; }

        [ElasticProperty(Name = "contact_fax")]
        public string ContactFax { get; set; }

        [ElasticProperty(Name = "contact_city")]
        public string ContactCity { get; set; }

        [ElasticProperty(Name = "contact_state")]
        public string ContactState { get; set; }

        [ElasticProperty(Name = "contact_country")]
        public string ContactCountry { get; set; }

        [ElasticProperty(Name = "contact_zip")]
        public string ContactZip { get; set; }

        [ElasticProperty(Name = "hashMD5")]
        public string HashMD5 { get; set; }

        [ElasticProperty(Name = "contact_company")]
        public string ContactCompany { get; set; }

        [ElasticProperty(Name = "contact_address2")]
        public string ContactAddress2 { get; set; }

        [ElasticProperty(Name = "contact_phone")]
        public string ContactPhone { get; set; }

        [ElasticProperty(Name = "contact_email")]
        public string ContactEmail { get; set; }

        [ElasticProperty(Name = "primary_kw_bag", Analyzer = "jobDefaultAnalyzer")]
        public IEnumerable<string> PrimaryKeywordBag { get; set; }


        [ElasticProperty(Name = "secondary_kw_bag", Analyzer = "jobDefaultAnalyzer")]
        public IEnumerable<string> SecondaryKeywordBag { get; set; }

        [ElasticProperty(Name = "pinned_start")]
        public DateTime? PinnedStart { get; set; }

        [ElasticProperty(Name = "pinned_expires")]
        public DateTime? PinnedExpires { get; set; }

        [ElasticProperty(Name = "pr_cat")]
        public bool? PrioritizeCategory { get; set; }

        [ElasticProperty(Name = "pr_loc")]
        public bool? PrioritizeLocation { get; set; }

        [ElasticProperty(Name = "pr_ttl")]
        public bool? PrioritizeTitle { get; set; }

        [ElasticProperty(Name = "pr_cmb")]
        public bool? PrioritizeCombine { get; set; }

        [ElasticProperty(Name = "pr_all")]
        public bool? PrioritizeAlways { get; set; }
    }


    [Serializable]
    public class ElasticsearchJobCategory
    {
        [ElasticProperty(Name = "id")]
        public long ID { get; set; }

        [ElasticProperty(Name = "job_id")]
        public long JobID { get; set; }

        [ElasticProperty(Name = "name")]
        public string Name { get; set; }

        [ElasticProperty(Name = "description")]
        public string Description { get; set; }

        [ElasticProperty(Name = "alias")]
        public string Alias { get; set; }

        [ElasticProperty(Name = "organization_hierarchy_id")]
        public int OrganizationHierarchyID { get; set; }

        [ElasticProperty(Name = "tenant_id")]
        public int TenantID { get; set; }

        [ElasticProperty(Name = "language_id")]
        public int LanguageID { get; set; }

        [ElasticProperty(Name = "category_group")]
        public bool IsCategoryGroup { get; set; }

        [ElasticProperty(Name = "group_Id")]
        public Nullable<long> GroupId { get; set; }
    }

    [Serializable]
    public class ElasticsearchCategoryLocation
    {
        [ElasticProperty(Name = "id_hash", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string IdHash { get; set; }

        [ElasticProperty(Name = "id_ancestors", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public List<string> IdAncestors { get; set; }

        [ElasticProperty(Name = "category_id", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CategoryID { get; set; }

        [ElasticProperty(Name = "category_name", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CategoryName { get; set; }

        [ElasticProperty(Name = "location_hierarchy_path", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string LocationHierarchyPath { get; set; }

        [ElasticProperty(Name = "location_country", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string LocationCountry { get; set; }

        [ElasticProperty(Name = "location_division_1", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string LocationDivision1 { get; set; }

        [ElasticProperty(Name = "location_city", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string LocationCity { get; set; }

        [ElasticProperty(Name = "category_group", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string IsCategoryGroup { get; set; }

        [ElasticProperty(Name = "group_Id", Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string GroupId { get; set; }

        [ElasticProperty(Name = "job_addresses")]
        public List<ElasticsearchJobAddress> JobAddresses { get; set; }
    }

    [Serializable]
    public class ElasticsearchJobBrand
    {
        [ElasticProperty(Name = "job_id")]
        public long JobID { get; set; }

        [ElasticProperty(Name = "organization_id")]
        public long OrganizationID { get; set; }

        [ElasticProperty(Name = "brand_name")]
        public string BrandName { get; set; }
    }
}
