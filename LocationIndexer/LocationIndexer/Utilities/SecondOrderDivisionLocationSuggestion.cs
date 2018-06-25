using Domain.ElasticsearchDocuments;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.Utilities
{
    public class SecondOrderDivisionLocationSuggestion : LocationSuggestion
    {
        #region Constructors

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public SecondOrderDivisionLocationSuggestion(ElasticsearchLocation location) : base(location) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// <param name="languageCode">Language Code</param>
        /// 
        public SecondOrderDivisionLocationSuggestion(ElasticsearchLocation location, string languageCode) : base(location, languageCode) { }
        #endregion Constructors

        public override List<string> Input
        {
            get
            {
                var input = this.Location.Suggest.Input.ToList();

                input.Add(this.Location.Division2);
                input.Add(string.Format("{0}, {1}", this.Location.Division2, this.Location.Division1));
                input.Add(string.Format("{0}, {1}", this.Location.Division2, this.Location.Division1Code));
                input.Add(string.Format("{0}, {1}, {2}", this.Location.Division2, this.Location.Division1, this.Location.Country));

                input = input.Distinct().ToList();

                return input.Select(x => NormalizeInputValue(x)).ToList();
            }
        }
    }
}
