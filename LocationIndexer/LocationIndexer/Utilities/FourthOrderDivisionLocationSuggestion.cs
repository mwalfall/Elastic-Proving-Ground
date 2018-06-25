using Domain.ElasticsearchDocuments;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.Utilities
{
    public class FourthOrderDivisionLocationSuggestion : LocationSuggestion
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public FourthOrderDivisionLocationSuggestion(ElasticsearchLocation location) : base(location) { }

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

                input.Add(this.Location.Division4);

                if (!string.IsNullOrWhiteSpace(this.Location.Division3))
                {
                    input.Add(string.Format("{0}, {1}, {2}", this.Location.Division4, this.Location.Division3, this.Location.Country));

                    if (!string.IsNullOrWhiteSpace(this.Location.Division2))
                        input.Add(string.Format("{0}, {1}, {2}, {3}", this.Location.Division4, this.Location.Division3, this.Location.Division2, this.Location.Country));

                    if (!string.IsNullOrWhiteSpace(this.Location.Division2) && !string.IsNullOrWhiteSpace(this.Location.Division1))
                        string.Format("{0}, {1}, {2}, {3}, {4}", this.Location.Division4, this.Location.Division3, this.Location.Division2, this.Location.Division1, this.Location.Country);

                    if (string.IsNullOrWhiteSpace(this.Location.Division2) && !string.IsNullOrWhiteSpace(this.Location.Division1))
                        string.Format("{0}, {1}, {2}, {3}", this.Location.Division4, this.Location.Division3, this.Location.Division1, this.Location.Country);
                }

                if (string.IsNullOrWhiteSpace(this.Location.Division3) && !string.IsNullOrWhiteSpace(this.Location.Division2))
                {
                    input.Add(string.Format("{0}, {1}, {2}", this.Location.Division4, this.Location.Division2, this.Location.Country));
                    if (!string.IsNullOrWhiteSpace(this.Location.Division1))
                        input.Add(string.Format("{0}, {1}, {2}, {3}", this.Location.Division4, this.Location.Division2, this.Location.Division1, this.Location.Country));
                }

                if (string.IsNullOrWhiteSpace(this.Location.Division3) && string.IsNullOrWhiteSpace(this.Location.Division2) && !string.IsNullOrWhiteSpace(this.Location.Division1))
                {
                    input.Add(string.Format("{0}, {1}, {2}", this.Location.Division4, this.Location.Division1, this.Location.Country));
                }

                input = input.Distinct().ToList();

                return input.Select(x => NormalizeInputValue(x)).ToList();
            }
        }
    }
}
