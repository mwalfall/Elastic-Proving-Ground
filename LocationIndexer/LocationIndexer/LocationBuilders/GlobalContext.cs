using Domain.Model;
using LocationIndexer.Services;
using LocationIndexer.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.LocationBuilders
{
    public class GlobalContext
    {
        private EnvironmentContext _environmentContext;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="environmentContext">EnvironmentContext object</param>
        public GlobalContext(EnvironmentContext environmentContext)
        {
            _environmentContext = environmentContext;
            InstantiateGlobalObjects();
        }
        #endregion Constructor

        #region Properties

        public EnvironmentContext EnvironmentContext { get { return _environmentContext; } }
        public List<LocationAlias> LocationAliases { get; set; }
        //public List<PreferredLocationNameSelection> PreferredLocationNameSelections { get; set; }

        #endregion Properties

        #region Services

        public IAdoClientService AdoClientService { get; set; }
        public ICountryService CountryService { get; set; }
        public ILocationAbbreviationService LocationAbbreviationService { get; set; }
        public ILocationCleanserService LocationCleanserService { get; set; }
        public ILocationSuggestionInputService LocationSuggestionInputService { get; set; }
        public IPreferredLocationNameService PreferredLocationNameService { get; set; }
        public IDuplicateSuggestionOutputResolverService DuplicateSuggestionOutputResolverService { get; set; }
        public IXmlDataService XmlDataService { get; set; }

        #endregion Services

        private void InstantiateGlobalObjects()
        {
            AdoClientService = new AdoClientService(EnvironmentContext.PlatformConnection);

            var resultSets = AdoClientService.GetMultiResultSets();

            var result = resultSets.SingleOrDefault(x => x.Item1.Equals("countrycode"));
            CountryService = new CountryService((List<string>) result.Item2);

            result = resultSets.SingleOrDefault(x => x.Item1.Equals("locationabbreviation"));
            LocationAbbreviationService = new LocationAbbreviationService((List<LocationAbbreviation>) result.Item2);

            result = resultSets.SingleOrDefault(x => x.Item1.Equals("locationcleanserdirective"));
            LocationCleanserService = new LocationCleanserService((List<LocationCleanserDirective>) result.Item2);

            result = resultSets.SingleOrDefault(x => x.Item1.Equals("locationsuggestioninput"));
            var result2 = resultSets.SingleOrDefault(x => x.Item1.Equals("charactertransformation"));
            LocationSuggestionInputService = new LocationSuggestionInputService((List<LocationSuggestionInput>) result.Item2, (List<CharacterTransformation>) result2.Item2);

            XmlDataService = new XmlDataService();

            result = resultSets.SingleOrDefault(x => x.Item1.Equals("preferredlocationnameselection"));
            result2 = resultSets.SingleOrDefault(x => x.Item1.Equals("locationname"));
            PreferredLocationNameService = new PreferredLocationNameService((List<PreferredLocationNameSelection>)result.Item2, (List<LocationName>)result2.Item2);

            DuplicateSuggestionOutputResolverService = new DuplicateSuggestionOutputResolverService();
        }
    }
}
