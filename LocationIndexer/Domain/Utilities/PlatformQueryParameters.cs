using Domain.Enums;
using System.Collections.Generic;
using System.Text;

namespace Domain.Utilities
{
    public static class PlatformQueryParameters
    {
        private static string _selectParameters = "SELECT [LOCATION].[ID] AS [ID], [LOCATION].[NAME] AS [NAME], [LOCATION].[CODE] AS [CODE], [LOCATION].[COUNTRYCODE] AS [COUNTRYCODE], [LOCATION].[TYPEID] AS [TYPEID], [LOCATION].[LATITUDE] AS [LATITUDE], [LOCATION].[LONGITUDE] AS [LONGITUDE], [LOCATION].[POPULATION] AS [POPULATION], [HIERARCHY].[LOCATIONPATH] AS [LOCATIONPATH], [HIERARCHY].PARENTHID.ToString() AS [PARENTLOCATIONPATH] ";
        private static string _fromClause = " FROM [DBO].[LOCATION] AS [LOCATION] INNER JOIN [DBO].[LOCATIONHIERARCHY] AS [HIERARCHY] ON [LOCATION].[ID] = [HIERARCHY].[ID] ";

        /// <summary>
        /// Returns a query string for the specified country and location type.
        /// </summary>
        /// <param name="locationType">LocationType enum</param>
        /// <param name="countryCode">Country Code</param>
        /// 
        public static string GetQuery(LocationType locationType, string countryCode)
        {
            var query = string.Empty;
            var whereClause = string.Format(" WHERE [LOCATION].[COUNTRYCODE] = '{0}' ", countryCode);

            switch (locationType)
            {
                case LocationType.Country:
                    return _selectParameters + _fromClause + whereClause + " AND [LOCATION].[TYPEID] = 0 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.FirstOrderDivision:
                    return _selectParameters + _fromClause + whereClause + " AND [LOCATION].[TYPEID] = 1 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.SecondOrderDivision:
                    return _selectParameters + _fromClause + whereClause + " AND [LOCATION].[TYPEID] = 2 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.ThirdOrderDivision:
                    return _selectParameters + _fromClause + whereClause + " AND [LOCATION].[TYPEID] = 3 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.FourthOrderDivision:
                    return _selectParameters + _fromClause + whereClause + " AND [LOCATION].[TYPEID] = 4 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.City:
                    return _selectParameters + _fromClause + whereClause + " AND [LOCATION].[TYPEID] = 5 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns a query string for the specified country and location type.
        /// </summary>
        /// <param name="locationType">LocationType enum</param>
        /// <param name="countryCode">Country Code</param>
        /// 
        public static string GetQuery(LocationType locationType, List<string> countryCodes)
        {
            var whereClause = new StringBuilder();
            var isFirst = true;
            whereClause.Append(" where ");

            foreach(var countryCode in countryCodes)
            {
                if (isFirst)
                    isFirst = false;
                else
                    whereClause.Append(" or ");

                whereClause.Append(string.Format(" [LOCATION].[COUNTRYCODE] = '{0}' ",countryCode));
            }

            switch (locationType)
            {
                case LocationType.Country:
                    return _selectParameters + _fromClause + whereClause.ToString() + " AND [LOCATION].[TYPEID] = 0 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.FirstOrderDivision:
                    return _selectParameters + _fromClause + whereClause.ToString() + " AND [LOCATION].[TYPEID] = 1 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.SecondOrderDivision:
                    return _selectParameters + _fromClause + whereClause.ToString() + " AND [LOCATION].[TYPEID] = 2 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.ThirdOrderDivision:
                    return _selectParameters + _fromClause + whereClause.ToString() + " AND [LOCATION].[TYPEID] = 3 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.FourthOrderDivision:
                    return _selectParameters + _fromClause + whereClause.ToString() + " AND [LOCATION].[TYPEID] = 4 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                case LocationType.City:
                    return _selectParameters + _fromClause + whereClause.ToString() + " AND [LOCATION].[TYPEID] = 5 ORDER BY [HIERARCHY].[LOCATIONPATH];";

                default:
                    return string.Empty;
            }
        }
    }
}
