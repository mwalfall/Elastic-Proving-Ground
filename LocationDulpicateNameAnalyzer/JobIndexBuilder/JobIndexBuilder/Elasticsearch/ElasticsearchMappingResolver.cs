using JobIndexBuilder.Domain;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobIndexBuilder.Elasticsearch
{
    public class ElasticsearchMappingResolver : IElasticsearchMappingResolver
    {
        public PutMappingDescriptor<T> Resolve<T>(IConnectionSettingsValues connectionSettings)
            where T : class
        {
            object mapping;
            var type = typeof(T);

            if (type == typeof(ElasticsearchJob))
            {
                mapping = GetJobsMapping(connectionSettings);
            }
            else if (type == typeof(ElasticsearchCategory))
            {
                mapping = GetCategoryMapping(connectionSettings);
            }
            else if (type == typeof(ElasticsearchLocation))
            {
                mapping = GetLocationsMapping(connectionSettings);
            }
            else
            {
                throw new ArgumentException("Could not resolve index mapping for type: " + type.ToString());
            }

            return (PutMappingDescriptor<T>)Convert.ChangeType(mapping, typeof(PutMappingDescriptor<T>));
        }

        private PutMappingDescriptor<ElasticsearchCategory> GetCategoryMapping(IConnectionSettingsValues connectionSettings)
        {
            var mapping = new PutMappingDescriptor<ElasticsearchCategory>(connectionSettings)
                .MapFromAttributes()
                .TimestampField(t => t.Enabled(true))
                .Properties(p => p
                    .MultiField(mf => mf
                        .Name(l => l.Name)
                        .Fields(fs => fs
                            .String(s => s.Name(l => l.Name).IndexAnalyzer("autocompletenative"))
                            .String(s => s.Name(l => l.Name.Suffix("raw")).IndexAnalyzer("autocomplete")))));

            return mapping;
        }

        private PutMappingDescriptor<ElasticsearchJob> GetJobsMapping(IConnectionSettingsValues connectionSettings)
        {
            var mapping = new PutMappingDescriptor<ElasticsearchJob>(connectionSettings)
                .MapFromAttributes()
                .DynamicTemplates(dtd => dtd
                    .Add(f => f
                        .Name("customStringFieldTemplate")
                        .PathMatch("custom_fields.*")
                        .MatchMappingType("string")
                        .Mapping(smd => smd
                            .MultiField(mf => mf
                                .Fields(fs => fs
                                    .String(js => js.Name("{name}").Index(FieldIndexOption.NotAnalyzed))
                                    .String(js => js.Name("lower").Index(FieldIndexOption.Analyzed).Analyzer("lowercase")))))))
                .TimestampField(t => t.Enabled(true))
                .Properties(props => props
                    .String(s => s
                        .Name(j => j.DescriptionHtml)
                        .Index(FieldIndexOption.No)
                        .Store(true))
                    .MultiField(mf => mf
                        .Name(j => j.Title)
                        .Fields(f => f
                            .String(js => js.Name(j => j.Title).Analyzer("job_title"))
                            .String(js => js.Name(j => j.Title.Suffix("primary")).Analyzer("jobDefaultWithDelimiterAnalyzer"))
                            .String(js => js.Name(j => j.Title.Suffix("secondary")).Analyzer("jobDefaultAnalyzer"))
                            .String(js => js.Name(j => j.Title.Suffix("suggest")).Analyzer("titleSuggestAnalyzer"))
                            .String(js => js.Name(j => j.Title.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))

                    .String(s => s
                        .Name(j => j.ExternalReferenceCode)
                        .Analyzer("lowercase"))

                    .MultiField(mf => mf
                        .Name(j => j.Description)
                        .Fields(f => f
                            .String(js => js.Name(j => j.Description).Analyzer("jobDefaultAnalyzer"))
                            .String(js => js.Name(j => j.Description.Suffix("raw")).Index(FieldIndexOption.No))))

                    .MultiField(mf => mf
                        .Name(j => j.Campaign)
                        .Fields(f => f
                            .String(js => js.Name(j => j.Campaign).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.Campaign.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.JobStatus)
                        .Fields(f => f
                            .String(js => js.Name(j => j.JobStatus).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.JobStatus.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.JobType)
                        .Fields(f => f
                            .String(js => js.Name(j => j.JobType).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.JobType.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.Industry)
                        .Fields(f => f
                            .String(js => js.Name(j => j.Industry).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.Industry.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.CompanyName)
                        .Fields(f => f
                            .String(js => js.Name(j => j.CompanyName).Index(FieldIndexOption.NotAnalyzed))//.String(js => js.Name(j => j.CompanyName).Analyzer("jobDefaultAnalyzer"))
                            .String(js => js.Name(j => j.CompanyName.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.Travel)
                        .Fields(f => f
                            .String(js => js.Name(j => j.Travel).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.Travel.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.SalaryRelocation)
                        .Fields(f => f
                            .String(js => js.Name(j => j.SalaryRelocation).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.SalaryRelocation.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.SalaryTime)
                        .Fields(f => f
                            .String(js => js.Name(j => j.SalaryTime).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.SalaryTime.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.IsManager)
                        .Fields(f => f
                            .String(js => js.Name(j => j.IsManager).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.IsManager.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.JobLevel)
                        .Fields(f => f
                            .String(js => js.Name(j => j.JobLevel).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.JobLevel.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.IsTelecommute)
                        .Fields(f => f
                            .String(js => js.Name(j => j.IsTelecommute).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.IsTelecommute.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .MultiField(mf => mf
                        .Name(j => j.HoursPerWeek)
                        .Fields(f => f
                            .String(js => js.Name(j => j.HoursPerWeek).Index(FieldIndexOption.NotAnalyzed))
                            .String(js => js.Name(j => j.HoursPerWeek.Suffix("lower")).Index(FieldIndexOption.Analyzed).Analyzer("lowercase"))))


                    .NestedObject<IDictionary<string, object>>(s => s.Name(j => j.CustomFields).MapFromAttributes())

                    .NestedObject<ElasticsearchJobCategory>(s => s
                        .Name(j => j.Categories.First())
                        .MapFromAttributes()
                        .Properties(cprops => cprops
                            .MultiField(mf => mf
                                .Name(c => c.Name)
                                .Fields(f => f
                                    .String(cs => cs.Name(c => c.Name).Index(FieldIndexOption.Analyzed))
                                    .String(cs => cs.Name(c => c.Name.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                             .MultiField(mf => mf
                                .Name(c => c.IsCategoryGroup)
                                .Fields(f => f
                                    .String(cs => cs.Name(c => c.IsCategoryGroup.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                             .MultiField(mf => mf
                                .Name(c => c.GroupId)
                                .Fields(f => f
                                    .String(cs => cs.Name(c => c.GroupId.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                                    ))



                    .NestedObject<ElasticsearchJobBrand>(s => s
                        .Name(j => j.Brands.First())
                        .MapFromAttributes()
                        .Properties(cprops => cprops
                            .MultiField(mf => mf
                                .Name(c => c.BrandName)
                                .Fields(f => f
                                    .String(cs => cs.Name(c => c.BrandName).Index(FieldIndexOption.Analyzed))
                                    .String(cs => cs.Name(c => c.BrandName.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))))

                    .NestedObject<IDictionary<String, String>>(s => s.Name(j => j.ApplyUrls).MapFromAttributes())

                    .NestedObject<ElasticsearchCategoryLocation>(s => s
                        .Name(j => j.CategoryLocations.First())
                        .MapFromAttributes()
                        )

                    .NestedObject<ElasticsearchLocation>(s => s
                        .Name(j => j.Locations.First())
                        .MapFromAttributes()
                        .Properties(lprops => lprops
                            .MultiField(mf => mf
                                .Name(l => l.HierarchyPath)
                                .Fields(f => f
                                    .String(ls => ls.Name(l => l.HierarchyPath).Index(FieldIndexOption.Analyzed).Analyzer("path"))
                                    .String(ls => ls.Name(l => l.HierarchyPath.Suffix("country_id")).Index(FieldIndexOption.Analyzed).Analyzer("country_path"))
                                    .String(ls => ls.Name(l => l.HierarchyPath.Suffix("region_id")).Index(FieldIndexOption.Analyzed).Analyzer("region_path"))
                                    .String(ls => ls.Name(l => l.HierarchyPath.Suffix("city_id")).Index(FieldIndexOption.Analyzed).Analyzer("city_path"))
                                    .String(ls => ls.Name(l => l.HierarchyPath.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                            .MultiField(mf => mf
                                .Name(l => l.Country)
                                .Fields(f => f
                                    .String(ls => ls.Name(l => l.Country).Index(FieldIndexOption.Analyzed))
                                    .String(ls => ls.Name(l => l.Country.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                            .MultiField(mf => mf
                                .Name(l => l.Division1)
                                .Fields(f => f
                                    .String(ls => ls.Name(l => l.Division1).Index(FieldIndexOption.Analyzed))
                                    .String(ls => ls.Name(l => l.Division1.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                            .MultiField(mf => mf
                                .Name(l => l.Division2)
                                .Fields(f => f
                                    .String(ls => ls.Name(l => l.Division2).Index(FieldIndexOption.Analyzed))
                                    .String(ls => ls.Name(l => l.Division2.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                            .MultiField(mf => mf
                                .Name(l => l.Division3)
                                .Fields(f => f
                                    .String(ls => ls.Name(l => l.Division3).Index(FieldIndexOption.Analyzed))
                                    .String(ls => ls.Name(l => l.Division3.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                            .MultiField(mf => mf
                                .Name(l => l.Division4)
                                .Fields(f => f
                                    .String(ls => ls.Name(l => l.Division4).Index(FieldIndexOption.Analyzed))
                                    .String(ls => ls.Name(l => l.Division4.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                            .MultiField(mf => mf
                                .Name(l => l.City)
                                .Fields(f => f
                                    .String(ls => ls.Name(l => l.City).Index(FieldIndexOption.Analyzed))
                                    .String(ls => ls.Name(l => l.City.Suffix("raw")).Index(FieldIndexOption.NotAnalyzed))))
                              .NestedObject<ElasticsearchJobAddress>(ad => ad
                                  .Name(j => j.JobAddresses.First())
                                  .MapFromAttributes()
                                  )
                                  )));

            return mapping;
        }

        private PutMappingDescriptor<ElasticsearchLocation> GetLocationsMapping(IConnectionSettingsValues connectionSettings)
        {
            var mapping = new PutMappingDescriptor<ElasticsearchLocation>(connectionSettings)
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
                        .Context(ct => ct.Category("country_ctx", ctx => ctx.Default("none").Path(path => path.CountryCtx))
                        )
                        .MaxInputLength(50)));

            return mapping;
        }

    }
}
