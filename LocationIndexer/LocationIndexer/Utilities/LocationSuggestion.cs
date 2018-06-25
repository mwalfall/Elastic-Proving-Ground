using Domain.ElasticsearchDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationIndexer.Utilities
{
    public abstract class LocationSuggestion
    {
        protected ElasticsearchLocation Location { get; set; }
        protected const int MaxInputLength = 50;
        protected string LanguageCode = "en";

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public LocationSuggestion(ElasticsearchLocation location)
        {
            this.Location = location;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// <param name="languageCode">Language Code</param>
        /// 
        public LocationSuggestion(ElasticsearchLocation location, string languageCode)
        {
            this.Location = location;
            LanguageCode = languageCode;
        }
        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Input property. Set by classes that inherit this class.
        /// </summary>
        public abstract List<string> Input { get; }

        /// <summary>
        /// The name that is displayed as the user enters characters.
        /// </summary>
        public virtual string Output
        {
            get { return LocationNameUtility.GetFormattedName(this.Location, LanguageCode); }
        }

        /// <summary>
        /// The data that is returned for the selected location for use by the client.
        /// </summary>
        public ElasticsearchLocation.Suggestion.SuggestionPayload Payload
        {
            get
            {
                var payload = new ElasticsearchLocation.Suggestion.SuggestionPayload
                {
                    ID = Location.ID,
                    Country = Location.Country,
                    HierarchyPath = Location.HierarchyPath,
                    CountryCode = Location.CountryCode,
                    Division1 = Location.Division1,
                    Division1Code = Location.Division1Code,
                    Division2 = Location.Division2,
                    Division2Code = Location.Division2Code,
                    Division3 = Location.Division3,
                    Division4 = Location.Division4,
                    City = Location.City,
                    Latitude = Location.Geometry.Latitude,
                    Longitude = Location.Geometry.Longitude,
                    TypeID = Location.TypeID
                };

                return payload;
            }
        }

        /// <summary>
        /// The weight of the location. Used for relevance.
        /// </summary>
        public virtual uint Weight
        {
            get
            {
                var weight = (this.Location.Population > 0) ? (uint)this.Location.Population : (uint)this.Location.TypeID;
                return weight;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates the ElasticseaerxhLocationSuggestion object.
        /// </summary>
        /// 
        public ElasticsearchLocation.Suggestion ToDocument()
        {
            return new ElasticsearchLocation.Suggestion
            {
                Input = this.Input,
                Output = this.Output,
                Payload = this.Payload,
                Weight = this.Weight
            };
        }

        /// <summary>
        /// Ensure that the submitted string is not greater than the MaxInputLength value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string NormalizeInputValue(string input)
        {
            return input.Length >= MaxInputLength
                ? input.Substring(0, MaxInputLength)
                : input;
        }
        #endregion Public Methods
    }
}
