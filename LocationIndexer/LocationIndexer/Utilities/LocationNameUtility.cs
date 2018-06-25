using Domain.ElasticsearchDocuments;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationIndexer.Utilities
{
    public class LocationNameUtility
    {
        /// <summary>
        /// Returns the output name what will be used for auto-complete.
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        public static string GetFormattedName(ElasticsearchLocation location, string languageCode)
        {
            if (location == null)
                throw new ArgumentNullException("location", "An object of type ElasticsearchLocation was not provided.");

            var type = (LocationType)location.TypeID;

            switch (type)
            {
                case LocationType.Country:
                    return GetCountryFormattedName(location);

                case LocationType.FirstOrderDivision:
                    return GetFirstOrderDivisionFormattedName(location);

                case LocationType.SecondOrderDivision:
                    return GetSecondOrderDivisionFormattedName(location, languageCode);

                case LocationType.ThirdOrderDivision:
                    return GetThirdOrderDivisionFormattedName(location);

                case LocationType.FourthOrderDivision:
                    return GetFourthOrderDivisionFormattedName(location);

                case LocationType.City:
                    return GetCityFormattedName(location, languageCode);

                default:
                    return string.Empty;
            }
        }

        #region Private Methods

        /// <summary>
        /// Country Name
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        ///
        private static string GetCountryFormattedName(ElasticsearchLocation location)
        {
            return location.Country;
        }

        /// <summary>
        /// First order division name
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        ///
        private static string GetFirstOrderDivisionFormattedName(ElasticsearchLocation location)
        {
            return string.Format("{0}, {1}", location.Division1, location.Country);
        }

        /// <summary>
        /// Second order division name
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        ///
        private static string GetSecondOrderDivisionFormattedName(ElasticsearchLocation location, string languageCode)
        {
            if (location.CountryCode.ToLower().Equals("us") && !languageCode.ToLower().Equals("zh"))
                return string.Format("{0}, {1}", location.Division2, location.Division1Code);

            return string.Join(", ", (new[] { location.Division2, location.Division1, location.Country }
                .Where(x => !string.IsNullOrEmpty(x) && !x.ContainsDigits())))
                .TrimEnd(',', ' ');
        }

        /// <summary>
        /// Third order Division Name
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// 
        private static string GetThirdOrderDivisionFormattedName(ElasticsearchLocation location)
        {
            if (location.CountryCode.ToLower().Equals("us"))
                return string.Format("{0}, {1}", location.Division3, location.Division1Code);

            if (location.CountryCode.ToLower().Equals("gb"))
                return string.Format("{0}, {1}, {2}", location.Division3, location.Division2, location.Division1);

            return string.Join(", ", (new[] { location.Division3, location.Division1, location.Country }
                .Where(x => !string.IsNullOrEmpty(x) && !x.ContainsDigits())))
                .TrimEnd(',', ' ');
        }

        /// <summary>
        /// Fourth Order Division Name
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        /// <returns></returns>
        private static string GetFourthOrderDivisionFormattedName(ElasticsearchLocation location)
        {
            if (location.CountryCode.ToLower().Equals("us"))
                return string.Format("{0}, {1}, {2}", location.Division4, location.Division2, location.Division1Code);

            if (location.CountryCode.ToLower().Equals("gb"))
                return string.Format("{0}, {1}, {2}, {3}", location.Division4, location.Division3, location.Division2, location.Division1);

            return string.Join(", ", (new[] { location.Division4, location.Division2, location.Division1, location.Country }
                .Where(x => !string.IsNullOrEmpty(x) && !x.ContainsDigits())))
                .TrimEnd(',', ' ');
        }

        /// <summary>
        /// City Name
        /// </summary>
        /// <param name="location">ElasticsearchLocation object</param>
        ///
        private static string GetCityFormattedName(ElasticsearchLocation location, string langaugeCode)
        {
            if (location.CountryCode.ToLower().Equals("us") && !langaugeCode.ToLower().Equals("zh"))
                return string.Format("{0}, {1}", location.City, location.Division1Code);

            return string.Join(", ", (new[] { location.City, location.Division1, location.Country }
                .Where(x => !string.IsNullOrEmpty(x))))
                .TrimEnd(',', ' ');
        }
        #endregion Private Methods
    }
}
