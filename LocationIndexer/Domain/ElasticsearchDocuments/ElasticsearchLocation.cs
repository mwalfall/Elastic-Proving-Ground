using Nest;
using System;
using System.Collections.Generic;

namespace Domain.ElasticsearchDocuments
{
    [Serializable]
    [ElasticType(Name = "location")]
    public class ElasticsearchLocation : ElasticsearchRootDocument
    {
        public ElasticsearchLocation()
        {
            this.Geometry = new ElasticsearchGeometry();
            this.AlternateNames = new List<string>();
            this.Suggest = new Suggestion();
            this.Suggest.Payload = new Suggestion.SuggestionPayload();
            this.Suggest.Input = new List<string>();
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
        public Suggestion Suggest { get; set; }

        [ElasticProperty(Name = "alternate_names")]
        public List<string> AlternateNames { get; set; }

        #region Mapping

        public static PutMappingDescriptor<ElasticsearchLocation> Mapping
        {
            get
            {
                var mapping = new PutMappingDescriptor<ElasticsearchLocation>(new ConnectionSettings())
                .MapFromAttributes()
                .TimestampField(t => t.Enabled(true))
                .Properties(p => p
                    .String(s => s
                        .Name(l => l.HierarchyPath)
                        .Index(FieldIndexOption.NotAnalyzed))
                    .MultiField(mf => mf
                        .Name(l => l.Country)
                        .Fields(fs => fs
                            .String(s => s.Name(l => l.Country).Index(FieldIndexOption.Analyzed))
                            .String(s => s.Name(l => l.Country.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                    .MultiField(mf => mf
                        .Name(l => l.Division1)
                        .Fields(fs => fs
                            .String(s => s.Name(l => l.Division1).Index(FieldIndexOption.Analyzed))
                            .String(s => s.Name(l => l.Division1.Suffix("shingle")).Analyzer("location_analyzer"))
                            .String(s => s.Name(l => l.Division1.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                    .MultiField(mf => mf
                        .Name(l => l.Division2)
                        .Fields(fs => fs
                            .String(s => s.Name(l => l.Division2).Index(FieldIndexOption.Analyzed))
                            .String(s => s.Name(l => l.Division2.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                    .MultiField(mf => mf
                        .Name(l => l.Division3)
                        .Fields(fs => fs
                            .String(s => s.Name(l => l.Division3).Index(FieldIndexOption.Analyzed))
                            .String(s => s.Name(l => l.Division3.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                    .MultiField(mf => mf
                        .Name(l => l.Division4)
                        .Fields(fs => fs
                            .String(s => s.Name(l => l.Division4).Index(FieldIndexOption.Analyzed))
                            .String(s => s.Name(l => l.Division4.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                    .MultiField(mf => mf
                        .Name(l => l.City)
                        .Fields(fs => fs
                            .String(s => s.Name(l => l.City.Suffix("lowercase")).Analyzer("lowercase"))
                            .String(s => s.Name(l => l.City.Suffix("shingle")).Analyzer("location_analyzer"))
                            .String(s => s.Name(l => l.City).Index(FieldIndexOption.Analyzed))
                            .String(s => s.Name(l => l.City.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                    .Completion(c => c
                        .Name(l => l.Suggest)
                        .IndexAnalyzer("suggestion_analyzer")
                        .SearchAnalyzer("suggestion_analyzer")
                        .Payloads(true)
                        .Context(ct => ct.Category("country_ctx", ctx => ctx.Default("none").Path(path => path.CountryCtx)))
                        .MaxInputLength(50)));

                return mapping;
            }
        }

        public static AnalysisDescriptor Analysis
        {
            get
            {
                var descriptor = new AnalysisDescriptor();

                descriptor.TokenFilters(cf => cf.Add("location_shingle", new ShingleTokenFilter { OutputUnigrams = false, OutputUnigramsIfNoShingles = true, MaxShingleSize = 3 }));

                var locationAnalyzer = new CustomAnalyzer
                {
                    Tokenizer = "standard",
                    Filter = new string[] { "standard", "lowercase", "location_shingle", "asciifolding" }
                };

                descriptor.Analyzers(a => a.Add("location_analyzer", locationAnalyzer));

                var suggestionAnalyzer = new CustomAnalyzer
                {
                    Tokenizer = "standard",
                    Filter = new string[] { "standard", "lowercase", "asciifolding" }
                };

                descriptor.Analyzers(a => a.Add("suggestion_analyzer", suggestionAnalyzer));

                var lowercaseAnalyzer = new CustomAnalyzer
                {
                    Tokenizer = "keyword",
                    Filter = new string[] { "lowercase" }
                };

                descriptor.Analyzers(a => a.Add("lowercase", lowercaseAnalyzer));

                return descriptor;
            }
        }

        #endregion

        #region Sub-Documents

        [Serializable]
        public class Suggestion
        {
            [ElasticProperty(Name = "input")]
            public List<string> Input { get; set; }

            [ElasticProperty(Name = "output")]
            public string Output { get; set; }

            [ElasticProperty(Name = "payload")]
            public SuggestionPayload Payload { get; set; }

            [ElasticProperty(Name = "weight")]
            public uint? Weight { get; set; }

            [Serializable]
            public class SuggestionPayload
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

                [ElasticProperty(Name = "division_2_code")]
                public string Division2Code { get; set; }

                [ElasticProperty(Name = "division_3")]
                public string Division3 { get; set; }

                [ElasticProperty(Name = "division_3_code")]
                public string Division3Code { get; set; }

                [ElasticProperty(Name = "division_4")]
                public string Division4 { get; set; }

                [ElasticProperty(Name = "division_4_code")]
                public string Division4Code { get; set; }

                [ElasticProperty(Name = "city")]
                public string City { get; set; }

                [ElasticProperty(Name = "type")]
                public string Type { get; set; }

                [ElasticProperty(Name = "type_id")]
                public int TypeID { get; set; }

                [ElasticProperty(Name = "lat")]
                public float Latitude { get; set; }

                [ElasticProperty(Name = "lon")]
                public float Longitude { get; set; }

                [ElasticProperty(Name = "hierarchy_id")]
                public long HierarchyID { get; set; }

                [ElasticProperty(Name = "weight")]
                public long Weight { get; set; }
            }
        }

        #endregion
    }
}
