using JobIndexBuilder.DTO;
using JobIndexBuilder.Services;
using System;

namespace JobIndexBuilder
{
    class Program
    {
        /// <summary>
        /// 
        /// INPUT PARAMETERS:
        ///     
        ///     -environment:       local, dev, qa, stg, prod
        ///                         DEFAULT: local
        ///     
        ///     -indexby:           alias (Use in index a group of indices that have the same alias)
        ///                         name (Use in situations where specific indices are to be reindexed.)
        ///                         DEFAULT: alias
        ///                         
        ///     -indexaliasvalues:  Comma separated list. Example: [jobs-en,jobs-de,jobs-pt]
        ///                         DEFAULT: [jobs-de,jobs-en,jobs-es,jobs-fr,jobs-pt,jobs-zh]
        ///                         
        ///     -indexnamevalues:   Comma separated list. Example: [jobs-en-117-08.29.16.18.14.05,jobs-en-2929-08.17.16.16.20.23,jobs-en-2961-08.24.16.15.00.53]
        ///                         DEFAULT: ""
        ///
        /// </summary>
        /// <param name="args">List of arguments.</param>
        /// 
        static void Main(string[] args)
        {
            var environmentContext = new EnvironmentContext(CommandLineOptionsService.GetCommandLineOptions(args));
            var reindexService = new JobReindexerService(new ElasticClientService(environmentContext));

            if(environmentContext.IsIndexByAlias)
                reindexService.ProcessIndicesByAlias(environmentContext.IndexAliasValues);
            else
                reindexService.ProcessIndicesByName(environmentContext.IndexNameValues);

            Console.WriteLine("---> All Processing Completed...");
            Environment.Exit(0);
        }
    }
}
