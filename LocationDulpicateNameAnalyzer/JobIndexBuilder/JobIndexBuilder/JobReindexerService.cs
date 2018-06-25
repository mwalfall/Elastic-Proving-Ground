using AutoMapper;
using JobIndexBuilder.Domain;
using JobIndexBuilder.Services;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobIndexBuilder
{
    public class JobReindexerService
    {
        private ElasticClientService _clientService;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientService">ElasticClientService object</param>
        public JobReindexerService(ElasticClientService clientService)
        {
            _clientService = clientService;

            Mapper.Initialize(cfg => {
                cfg.CreateMap<ElasticsearchJobV1, ElasticsearchJob>();
                cfg.CreateMap<ElasticsearchLocationV1, ElasticsearchLocation>();
            });
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Entrypoin to process indices by alias group name
        /// </summary>
        /// <param name="aliases">List of alias group names. Ex. 'jobs-en, jobs-de'</param>
        /// 
        public void ProcessIndicesByAlias(List<string> aliases)
        {
            foreach( var alias in aliases)
            {
                Console.WriteLine(string.Format("Building index for: {0}.", alias));
                BuildIndicesByAlias(alias);

                Console.WriteLine(string.Format("--->Switching aliases."));
                SwitchAliases(alias);

                Console.WriteLine(string.Format("---> Processing of {0} Completed...", alias));
                Console.WriteLine("--->");
            }
        }

        /// <summary>
        /// Entrypoint to process indices by name.
        /// </summary>
        /// <param name="names">Index names. Ex. jobs-en-117-08.29.16.18.14.05 </param>
        /// 
        public void ProcessIndicesByName(List<string> names)
        {
            BuildIndicesByName(names);
            Console.WriteLine("--->");
            Console.WriteLine("---> Processing Completed...");
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Switch aliases for job indices
        /// </summary>
        /// <param name="aliasGroupName"></param>
        /// 
        private void SwitchAliases(string aliasGroupName)
        {
            var jobIndices = _clientService.GetIndicesPointingToAlias(aliasGroupName).ToArray();

            foreach (var jobIndex in jobIndices)
            {
                var parts = jobIndex.Split('-');

                var exisitingGroupAlias = string.Format("{0}-{1}", parts[0], parts[1]);
                var existingClientAlias = string.Format("{0}-{1}-{2}", parts[0], parts[1], parts[2]);

                var newClientAlias = existingClientAlias + "-new";
                var newGroupAlias = exisitingGroupAlias + "-new";

                var newIndex = _clientService.GetIndicesPointingToAlias(newClientAlias).ToArray();

                if (newIndex.Count() != 1)
                {
                    Console.WriteLine(string.Format("ERROR ----> New index {0} was not found.", newClientAlias));
                    continue;
                }

                var oldIndex = _clientService.GetIndicesPointingToAlias(existingClientAlias).ToArray();
                if (oldIndex.Count() != 1)
                {
                    Console.WriteLine(string.Format("ERROR ----> Existing index {0} was not found.", existingClientAlias));
                    continue;
                }

                _clientService.RemoveAlias(oldIndex[0], existingClientAlias, exisitingGroupAlias);

                _clientService.AddAlias(newIndex[0], exisitingGroupAlias);
                _clientService.AddAlias(newIndex[0], existingClientAlias);
                _clientService.RemoveAlias(newIndex[0], newClientAlias, newGroupAlias);
            }
        }
        
        /// <summary>
        /// When building indices by name remove the alias of the existing index and add the alias to the new index.
        /// </summary>
        /// <param name="newIndexName">New Job Index</param>
        /// <param name="existingIndexName">Existing Job Index</param>
        /// 
        private void SwitchAlias(string newIndexName, string existingIndexName)
        {
            var parts = newIndexName.Split('-');

            var exisitingGroupAlias = string.Format("{0}-{1}", parts[0], parts[1]);
            var existingClientAlias = string.Format("{0}-{1}-{2}", parts[0], parts[1], parts[2]);

            Console.WriteLine("---> Switching alias...");

            _clientService.RemoveAlias(existingIndexName, existingClientAlias, exisitingGroupAlias);

            _clientService.AddAlias(newIndexName, exisitingGroupAlias);
            _clientService.AddAlias(newIndexName, existingClientAlias);
        }

        /// <summary>
        /// Reindex all indices that have the provided group alias.
        /// </summary>
        /// <param name="indexAlias">Group Alias Name (Ex. jobs-en)</param>
        /// 
        private void BuildIndicesByAlias(string indexAlias)
        {
            Console.WriteLine("---> Indexing begins...");
            Console.WriteLine(string.Format("---> One moment please... Obtaining indices with the alias: {0}.", indexAlias));
            var jobIndices = _clientService.GetIndicesPointingToAlias(indexAlias).ToArray();

            var aliasGroupName = indexAlias + "-new";
            foreach (var jobIndex in jobIndices)
            {
                var parts = jobIndex.Split('-');
                var newIndexName = string.Format("{0}-{1}-{2}-{3}", parts[0], parts[1], parts[2], DateTime.Now.ToString("MM.dd.yy.HH.mm.ss"));
                var clientAliasName = string.Format("{0}-{1}-{2}-new", parts[0], parts[1], parts[2]);

                Console.WriteLine(string.Format("---> Processing index : {0}-{1}-{2}", parts[0], parts[1], parts[2]));
                _clientService.CreateIndex<ElasticsearchJob>(newIndexName, 3, 0, true);

                var _esClient = _clientService.GetClient();
                var response = _esClient.Search<ElasticsearchJobV1>(l => l
                    .Index(jobIndex)
                    .Size(1000000)
                    .Query(q => q.MatchAll()));

                if (response.Total == 0)
                {
                    _clientService.AddAlias(newIndexName, aliasGroupName);
                    _clientService.AddAlias(newIndexName, clientAliasName);
                    continue;
                }

                var processedDocuments = new List<ElasticsearchJob>();
                
                foreach(var document in response.Documents)
                {
                    processedDocuments.Add(MapJob(document));
                }

                IndexBulk(processedDocuments, newIndexName);

                _clientService.AddAlias(newIndexName, aliasGroupName);
                _clientService.AddAlias(newIndexName, clientAliasName);
            }
        }

        /// <summary>
        /// Build indices using the provided index names.
        /// </summary>
        /// <param name="existingIndices">List of index names</param>
        /// 
        private void BuildIndicesByName(List<string> existingIndices)
        {
            /*
                [jobs-en-117-08.29.16.18.14.05,jobs-en-2929-08.17.16.16.20.23,jobs-en-2961-08.24.16.15.00.53,jobs-en-3958-08.29.16.18.16.05,jobs-en-9-08.26.16.17.47.34]
            */

            Console.WriteLine("---> Indexing begins...");

            foreach (var existingIndex in existingIndices)
            {
                var parts = existingIndex.Split('-');
                var newIndexName = string.Format("{0}-{1}-{2}-{3}", parts[0], parts[1], parts[2], DateTime.Now.ToString("MM.dd.yy.HH.mm.ss"));
                var indexAlias = string.Format("{0}-{1}", parts[0], parts[1]);
                var aliasGroupName = indexAlias + "-new";
                var clientAliasName = string.Format("{0}-{1}-{2}-new", parts[0], parts[1], parts[2]);

                Console.WriteLine(string.Format("---> Processing index : {0}-{1}-{2}", parts[0], parts[1], parts[2]));

                _clientService.CreateIndex<ElasticsearchJob>(newIndexName, 3, 0, true);

                var _esClient = _clientService.GetClient();
                var response = _esClient.Search<ElasticsearchJobV1>(l => l
                    .Index(existingIndex)
                    .Size(1000000)
                    .Query(q => q.MatchAll()));

                if (response.Total == 0)
                {
                    _clientService.AddAlias(newIndexName, aliasGroupName);
                    _clientService.AddAlias(newIndexName, clientAliasName);
                    continue;
                }

                var processedDocuments = new List<ElasticsearchJob>();

                foreach (var document in response.Documents)
                {
                    processedDocuments.Add(MapJob(document));
                }

                IndexBulk(processedDocuments, newIndexName);

                SwitchAlias(newIndexName, existingIndex);
            }
        }

 

        /// <summary>
        /// Perform bulk index of jobs
        /// </summary>
        /// <param name="documents">Job documents</param>
        /// <param name="index">Index name</param>
        /// 
        private void IndexBulk(IEnumerable<ElasticsearchJob> documents, string index)
        {
            var descriptor = new BulkDescriptor();

            foreach (var aDocument in documents)
            {
                descriptor.Index<ElasticsearchJob>(d => d
                    .Index(index)
                    .Document(aDocument));
            }

            var _esClient = _clientService.GetClient();
            var response = _esClient.Bulk(d => descriptor);
            if (response.Errors)
            {
                var errors = response.ItemsWithErrors.ToList();
                throw new Exception("Unable to index all documents. Error occurred during Bulk index operation. Returned Message: " + errors[0].Error);
            }
        }

        /// <summary>
        /// Map the jobs
        /// </summary>
        /// <param name="job">Job in existing format</param>
        /// <returns>Job in new format</returns>
        /// 
        private ElasticsearchJob MapJob(ElasticsearchJobV1 job)
        {
            var newJob = Mapper.Map<ElasticsearchJob>(job);

            newJob.CategoryLocations = job.CategoryLocations;
            newJob.Brands = job.Brands;
            newJob.Categories = job.Categories;

            var locations = Mapper.Map<List<ElasticsearchLocation>>(job.Locations);
            newJob.Locations = locations;

            return newJob;
        }
        #endregion Private Methods
    }
}
