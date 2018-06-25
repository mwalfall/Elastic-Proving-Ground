using Domain.ElasticsearchDocuments;
using System.Collections.Generic;
using System.Linq;


namespace LocationIndexer.Utilities
{
    public class CountryLocationSuggestion : LocationSuggestion
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public CountryLocationSuggestion(ElasticsearchLocation location) : base(location) { }

        #endregion Constructor

        /// <summary>
        /// Populates the Input property. 
        /// NOTE: All other properties are populated by the LocationSuggestion base class.
        /// </summary>
        public override List<string> Input
        {
            get
            {
                var input = this.Location.Suggest.Input;
                input = input.Distinct().ToList();

                return input.Select(x => NormalizeInputValue(x)).ToList();
            }
        }
    }
}
