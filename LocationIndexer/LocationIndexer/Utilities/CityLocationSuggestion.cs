using Domain.ElasticsearchDocuments;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.Utilities
{
    public class CityLocationSuggestion : LocationSuggestion
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public CityLocationSuggestion(ElasticsearchLocation location) : base(location) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// <param name="alternateNames">List of preferred names for the location</param>
        /// 
        public CityLocationSuggestion(ElasticsearchLocation location, string languageCode) : base(location, languageCode) { }

        #endregion Constructors

        /// <summary>
        /// Populates the Input property. 
        /// NOTE: All other properties are populated by the LocationSuggestion base class.
        /// </summary>
        /// 
        public override List<string> Input
        {
            get
            {
                var input = this.Location.Suggest.Input.ToList();

                input.Add(this.Location.City);
                input.Add(string.Format("{0}, {1}", this.Location.City, this.Location.Division1));
                input.Add(string.Format("{0}, {1}", this.Location.City, this.Location.Division1Code));
                input.Add(string.Format("{0}, {1}", this.Location.City, this.Location.Country));
                input.Add(string.Format("{0}, {1}", this.Location.City, this.Location.CountryCode));

                // Add the altenate names.
                input = input.Distinct().ToList();

                return input.Select(x => NormalizeInputValue(x)).ToList();
            }
        }
    }
}
