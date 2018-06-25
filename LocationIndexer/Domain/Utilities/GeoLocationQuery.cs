using Domain.Enums;

namespace Domain.Utilities
{
    public class GeoLocationQuery
    {
        private static string _selectParameters = "SELECT DISTINCT [ID], [Name] AS [NAME], [Latitude] AS[LATITUDE] , [Longitude] AS[LONGITUDE], [CountryCode] AS [COUNTRYCODE], [Admin1Code] AS [ADMIN1CODE], [Admin2Code] AS [ADMIN2CODE] , [Admin3Code] AS [ADMIN3CODE], [Admin4Code] AS [ADMIN4CODE] ,[Population] AS [POPULATION] ";
        private static string _fromClause = " FROM [GeoNames].[Location]";
        private static string _countryFromClause = " and (([FeatureCode] = N'PCLI') OR ([FeatureCode] = N'PCL' AND [CountryCode] IN (N'GG', N'JE', N'IM')) OR ([FeatureCode] = N'PCLD' AND [CountryCode] IN (N'AX', N'AI', N'BM', N'IO', N'VG', N'KY', N'MP', N'FK', N'FO', N'GI', N'GL', N'GP', N'GU', N'GF', N'MQ', N'YT', N'MS', N'NC', N'PN', N'PF', N'PR', N'RE', N'SH', N'PM', N'GS', N'CX', N'CC', N'NF', N'TK', N'TC', N'UM', N'VI', N'WF')) OR([FeatureCode] = N'PCLF' AND [CountryCode] IN (N'FM', N'PW')) OR ([FeatureCode] = N'PCLIX' AND [CountryCode] IN (N'AW', N'BQ', N'CW', N'BL', N'MF', N'SX', N'TF')) OR ([FeatureCode] = N'PCLS' AND [CountryCode] IN (N'CK', N'HK', N'MO', N'NU', N'PS')) OR ([FeatureCode] = N'PCLF' AND [CountryCode] = N'MH') OR ([FeatureCode] = N'TERR' AND [CountryCode] IN (N'AS', N'AQ', N'SJ', N'EH'))) AND ([CountryCode] IS NOT NULL AND [Name] IS NOT NULL)";
        private static string _admin1FromClause = " and (([FeatureCode] = N'ADM1')) AND ([CountryCode] IS NOT NULL AND [Admin1Code] IS NOT NULL AND [Name] IS NOT NULL)";
        private static string _admin2FromClause = " and (([FeatureCode] = N'ADM2')) AND ([CountryCode] IS NOT NULL AND [Admin1Code] IS NOT NULL AND [Admin2Code] IS NOT NULL AND [Name] IS NOT NULL);";
        private static string _admin3FromClause = " and (([FeatureCode] = N'ADM3')) AND ([CountryCode] IS NOT NULL AND [Admin1Code] IS NOT NULL AND [Admin2Code] IS NOT NULL AND [Admin3Code] IS NOT NULL AND [Name] IS NOT NULL);";
        private static string _admin4FromCluase = " and (([FeatureCode] = N'ADM4')) AND ([CountryCode] IS NOT NULL AND [Admin1Code] IS NOT NULL AND [Admin2Code] IS NOT NULL AND [Admin3Code] IS NOT NULL AND [Admin4Code] IS NOT NULL AND [Name] IS NOT NULL);";
        private static string _cityClause = "";

        /// <summary>
        /// Returns a query string for the specified country and location type.
        /// </summary>
        /// <param name="locationType">LocationType enum</param>
        /// <param name="countryCode">Country Code</param>
        /// 
        public static string GetQuery(LocationType locationType, string countryCode)
        {
            var query = string.Empty;
            var whereClause = string.Format(" WHERE CountryCode = '{0}'", countryCode);

            switch (locationType)
            {
                case LocationType.Country:
                    return _selectParameters + _fromClause + whereClause + _countryFromClause;

                case LocationType.FirstOrderDivision:
                    return _selectParameters + _fromClause + whereClause + _admin1FromClause;

                case LocationType.SecondOrderDivision:
                    return _selectParameters + _fromClause + whereClause + _admin2FromClause;

                case LocationType.ThirdOrderDivision:
                    return _selectParameters + _fromClause + whereClause + _admin3FromClause;

                case LocationType.FourthOrderDivision:
                    return _selectParameters + _fromClause + whereClause + _admin4FromCluase;

                case LocationType.City:
                    return _selectParameters + _fromClause + whereClause + _cityClause;

                default:
                    return string.Empty;
            }
        }
    }
}
