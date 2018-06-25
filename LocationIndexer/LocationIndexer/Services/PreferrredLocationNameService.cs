using Domain.ElasticsearchDocuments;
using Domain.Model;
using LocationIndexer.LocationBuilders;
using LocationIndexer.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.Services
{
    public class PreferredLocationNameService : IPreferredLocationNameService
    {
        private List<PreferredLocationNameSelection> _preferredNameSelectionList;
        private List<LocationName> _locationCustomNameList;
        private List<LocationAlias> _locationAliases;

        #region Constructor

        public PreferredLocationNameService(List<PreferredLocationNameSelection> preferredNameSelectionList, List<LocationName> locationCustomNameList)
        {
            _preferredNameSelectionList = preferredNameSelectionList;
            _locationCustomNameList = locationCustomNameList; 
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Entrypoint to determine the name to use for a location. The ordering is (short-circuit) as follows: 
        /// Custom Name - Short Name - Preferred Name - General Name (in specified language) - Default Name
        /// </summary>
        /// <param name="esDocument">ElasticsearchLocatio object</param>
        /// <param name="globalContext">GlobalContext object</param>
        /// 
        public ElasticsearchLocation SetPreferredName(ElasticsearchLocation esDocument, GlobalContext globalContext)
        {
            _locationAliases = globalContext.LocationAliases;

            // Get custom selection for the specified location.
            var selection = _preferredNameSelectionList.SingleOrDefault(x => x.SourceID == esDocument.ID && x.LanguageCode.ToLower().Equals(globalContext.EnvironmentContext.IndexLanguage.ToLower()));
            if (selection != null)
            {
                var name = GetCustomNameSelection(selection, esDocument.ID);
                if (!string.IsNullOrWhiteSpace(name))
                    return SetLocationsPreferredName(esDocument, name);
            }

            // The short name for the location in the specified language.
            var shortNameForLocation = _locationAliases.FirstOrDefault(x => x.SourceId == esDocument.ID && x.LanguageCode.ToLower().Equals(globalContext.EnvironmentContext.IndexLanguage.ToLower()) && x.IsShortName);
            if (shortNameForLocation != null)
                return SetLocationsPreferredName(esDocument, shortNameForLocation.Name);

            // The preferred name for the location in the specified language.
            var preferredNameForLocation = _locationAliases.FirstOrDefault(x => x.SourceId == esDocument.ID && x.LanguageCode.ToLower().Equals(globalContext.EnvironmentContext.IndexLanguage.ToLower()) && x.IsPreferredName);
            if (preferredNameForLocation != null)
                return SetLocationsPreferredName(esDocument, preferredNameForLocation.Name);

            // The general name of the location in the specified language.
            var languageNameForlocation = _locationAliases.FirstOrDefault(x => x.SourceId == esDocument.ID && x.LanguageCode.ToLower().Equals(globalContext.EnvironmentContext.IndexLanguage.ToLower()) && !x.IsPreferredName && !x.IsColloquial && !x.IsHistoric && !x.IsShortName);
            if (languageNameForlocation != null)
                return SetLocationsPreferredName(esDocument, languageNameForlocation.Name);

            return esDocument;
        }

        /// <summary>
        /// Sets to location alias list for the countries being processed.
        /// </summary>
        /// <param name="locationAliases">List of LocationAlias objects</param>
        /// 
        public void SetLocationAliases(List<LocationAlias> locationAliases)
        {
            _locationAliases = locationAliases;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// This method provides a means to explicitly indicate what name is to used fdr a location. Overrides rules in main entrypoint method.
        /// </summary>
        /// <param name="selection">PreferredLocationNameSelectio object</param>
        /// <param name="sourceId">Geonames Source Id</param>
        /// 
        /// 
        private string GetCustomNameSelection(PreferredLocationNameSelection selection, long sourceId)
        {
            if (selection.UseCustomName)
            {
                var location = _locationCustomNameList.FirstOrDefault(x => x.SourceID == sourceId &&
                                                                           x.CountryCode.ToLower().Equals(selection.CountryCode.ToLower()) &&
                                                                           x.LanguageCode.ToLower().Equals(selection.LanguageCode.ToLower()));
                return (location != null) ? location.Name : null;
            }

            if (selection.UseShortName)
            {
                var location = _locationAliases.FirstOrDefault(x => x.SourceId == sourceId && x.IsShortName);
                return (location != null) ? location.Name : null;
            }

            if (selection.UsePreferredName)
            {
                var location = _locationAliases.FirstOrDefault(x => x.SourceId == sourceId && x.IsPreferredName);
                return (location != null) ? location.Name : null;
            }

            return null;
        }

        /// <summary>
        /// Set the previously determined preferred name for the ElasticsearchLocation object.
        /// </summary>
        /// <param name="esDocument">ElasticsearchLocation object</param>
        /// <param name="location">Messages.Location object</param>
        /// 
        private ElasticsearchLocation SetLocationsPreferredName(ElasticsearchLocation esDocument, string preferredName)
        {
            switch (esDocument.TypeID)
            {
                case 0:
                    esDocument.Country = preferredName;
                    return esDocument;
                case 1:
                    esDocument.Division1 = preferredName;
                    return esDocument;
                case 2:
                    esDocument.Division2 = preferredName;
                    return esDocument;
                case 3:
                    esDocument.Division3 = preferredName;
                    return esDocument;
                case 4:
                    esDocument.Division4 = preferredName;
                    return esDocument;
                case 5:
                    esDocument.City = preferredName;
                    return esDocument;
                default:
                    return esDocument;
            }
        }
        #endregion Private Methods
    }
}
