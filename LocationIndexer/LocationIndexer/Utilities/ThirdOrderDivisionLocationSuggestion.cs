using Domain.ElasticsearchDocuments;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.Utilities
{
    public class ThirdOrderDivisionLocationSuggestion : LocationSuggestion
    {
        private readonly List<string> _alternateNames = new List<string>();

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public ThirdOrderDivisionLocationSuggestion(ElasticsearchLocation location) : base(location) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// <param name="alternateNames">List containing preferred names</param>
        /// 
        public ThirdOrderDivisionLocationSuggestion(ElasticsearchLocation location, List<string> alternateNames) : base(location)
        {
            _alternateNames = alternateNames.Distinct().ToList();
        }
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

                input.Add(this.Location.Division3);
                input.Add(string.Format("{0}, {1}", this.Location.Division3, this.Location.Division2));
                input.Add(string.Format("{0}, {1}", this.Location.Division3, this.Location.Division2Code));
                input.Add(string.Format("{0}, {1}, {2}", this.Location.Division3, this.Location.Division2, this.Location.Country));
                input.Add(string.Format("{0}, {1}, {2}, {3}", this.Location.Division3, this.Location.Division2, this.Location.Division1, this.Location.Country));

                input = input.Distinct().ToList();

                return input.Select(x => NormalizeInputValue(x)).ToList();
            }
        }
    }
}
