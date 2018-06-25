using Domain.ElasticsearchDocuments;
using System.Collections.Generic;
using System.Linq;


namespace LocationIndexer.Utilities
{
    public class FirstOrderDivisionLocationSuggestion : LocationSuggestion
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation</param>
        /// 
        public FirstOrderDivisionLocationSuggestion(ElasticsearchLocation location) : base(location) { }

        #endregion Constructors

        /// <summary>
        /// Populates the Input property. 
        /// NOTE: All other properties are populated by the LocationSuggestion base class.
        /// </summary>
        public override List<string> Input
        {
            get
            {
                var input = this.Location.Suggest.Input.ToList();

                input.Add(this.Location.Division1);
                input.Add(string.Format("{0}, {1}", this.Location.Division1, this.Location.Country));
                input.Add(string.Format("{0}, {1}", this.Location.Division1, this.Location.CountryCode));

                input = input.Distinct().ToList();

                return input.Select(x => NormalizeInputValue(x)).ToList();
            }
        }
    }
}
