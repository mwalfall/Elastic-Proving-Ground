using Domain.ElasticsearchDocuments;
using Domain.Model;
using LocationIndexer.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocationIndexer.Services
{
    public class LocationSuggestionInputService : ILocationSuggestionInputService
    {
        private List<LocationSuggestionInput> _locationSuggestionInputs = new List<LocationSuggestionInput>();
        private List<CharacterTransformation> _characterTransformations = new List<CharacterTransformation>();

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adoClientService">IAdoClientService object</param>
        /// 
        public LocationSuggestionInputService(List<LocationSuggestionInput> locationSuggestionInputs, List<CharacterTransformation> characterTransformations)
        {
            _locationSuggestionInputs = locationSuggestionInputs;
            _characterTransformations = characterTransformations;
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Takes in a location object and adds suggestions to Suggest.Input list.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public ElasticsearchLocation AddInputs(ElasticsearchLocation location)
        {
            var suggestions = GetSuggestions(location.ID);
            var transformations = GetSuggestionWithCharacterTransformation(location.CountryCode, GetLocationName(location));
            if (!string.IsNullOrWhiteSpace(transformations))
                suggestions.Add(transformations);

            if (!suggestions.Any())
                return location;

            var suggestionInputs = location.Suggest.Input.ToList();
            suggestionInputs.AddRange(suggestions);

            suggestionInputs.AddRange(location.AlternateNames);
            location.Suggest.Input = suggestionInputs.Distinct().ToList();

            return location;
        }
        /// <summary>
        /// Get the input suggestions for the specified source id.
        /// </summary>
        /// <param name="sourceId">Source Id</param>
        /// 
        public List<string> GetSuggestions(long sourceId)
        {
            return sourceId < 1
                ? new List<string>()
                : _locationSuggestionInputs.Where(x => x.SourceId == sourceId).Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Transform characters so that locations can be found using default letters.
        /// For example ö -> oe, ü -> ue.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="locationName">Location Name</param>
        /// <returns>Transformed name for the location. Otherwise, mptry string if transformation did not occur.</returns>
        /// 
        public string GetSuggestionWithCharacterTransformation(string countryCode, string locationName)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(locationName))
                return string.Empty;

            var transformations = _characterTransformations.Where(x => x.CountryCode.ToLower().Equals(countryCode.ToLower()));
            if (!transformations.Any())
                return string.Empty;


            var sb = new StringBuilder(locationName.ToLower());
            foreach (var item in transformations)
            {
                sb.Replace(item.Character, item.Replacement);
            }

            return (locationName.ToLower().Equals(sb.ToString()))
                ? string.Empty
                : sb.ToString();
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Provides the correct location name for processing.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        private string GetLocationName(ElasticsearchLocation location)
        {
            switch (location.TypeID)
            {
                case 0:
                    return location.Country;
                case 1:
                    return location.Division1;
                case 2:
                    return location.Division2;
                case 3:
                    return location.Division3;
                case 4:
                    return location.Division4;
                case 5:
                    return location.City;
                default:
                    return string.Empty;
            }
        }
        #endregion Private Methods
    }
}
