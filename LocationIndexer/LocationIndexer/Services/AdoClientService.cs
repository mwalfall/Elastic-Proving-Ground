using Domain.Model;
using LocationIndexer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using Domain.Utilities;
using LocationIndexer.Utilities;

namespace LocationIndexer.Services
{
    public class AdoClientService : IAdoClientService
    {
        ConnectionStringSettings _sqlPlatformConnectionString = null;

        #region Constructor

        public AdoClientService(ConnectionStringSettings platformConnectionString)
        {
            if (platformConnectionString == null)
                throw new NullReferenceException("A Platform Sql Connection String was not provided.");

            _sqlPlatformConnectionString = platformConnectionString;
        }
        #endregion 

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<LocationView> GetLocations(string query)
        {
            var geoLocations = new List<LocationView>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            geoLocations.Add(new LocationView(reader));
                        }
                        reader.Close();
                    }
                }
            }
            return geoLocations;
        }

        /// <summary>
        /// Get Location Aliases.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="languageCodes">Language Codes from the app.config</param>
        /// 
        public List<LocationAlias> GetLocationAliases(string countryCode, List<string> languageCodes)
        {
            var selectStatement = "SELECT [Platform].[dbo].[LocationAlias].[SOURCEID] AS SOURCEID " +
                                ",[Platform].[dbo].[LocationAlias].[NAME] AS NAME " +
                                ",[LANGUAGECODE] AS LANGUAGECODE " +
                                ",[ISPREFERREDNAME] AS ISPREFERREDNAME " +
                                ",[ISSHORTNAME] AS ISSHORTNAME " +
                                "FROM [Platform].[dbo].[LocationAlias] " +
                                "INNER JOIN [Platform].[dbo].[Location] " +
                                "ON [Platform].[dbo].[LocationAlias].[SOURCEID] = [Platform].[dbo].[Location].[ID] ";

            var whereCountryClause = string.Format("where [Platform].[dbo].[Location].[CountryCode] = '{0}' ", countryCode);

            var whereLanguageCodeClause = new StringBuilder();
            if (languageCodes.Any())
            {
                var isFirst = true;
                whereLanguageCodeClause.Append(" and (");
                foreach(var languageCode in languageCodes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        whereLanguageCodeClause.Append(" OR ");
                    if (languageCode.ToLower().Equals("null"))
                        whereLanguageCodeClause.Append(string.Format("[Platform].[dbo].[LocationAlias].[LanguageCode] is null"));
                    else
                        whereLanguageCodeClause.Append(string.Format("[Platform].[dbo].[LocationAlias].[LanguageCode] = '{0}'", languageCode));
                }
                whereLanguageCodeClause.Append(")");
            }

            var query = selectStatement + whereCountryClause + whereLanguageCodeClause.ToString();

            var locationAliases = new List<LocationAlias>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["SOURCEID"] != DBNull.Value && reader["NAME"] != DBNull.Value && reader["LANGUAGECODE"] != DBNull.Value && reader["ISPREFERREDNAME"] != DBNull.Value && reader["ISSHORTNAME"] != DBNull.Value)
                            {
                                locationAliases.Add(new LocationAlias
                                {
                                    SourceId = (long)reader["SOURCEID"],
                                    Name = reader["NAME"].ToString(),
                                    LanguageCode = reader["LANGUAGECODE"].ToString(),
                                    IsPreferredName = (bool) reader["ISPREFERREDNAME"],
                                    IsShortName = (bool) reader["ISSHORTNAME"]
                                });
                            }
                        }
                        reader.Close();
                    }
                }

                return locationAliases.Where(x => !x.Name.ContainsDigits()).ToList(); ;
            }
        }

        /// <summary>
        /// Get unique location names
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="languageCode">Language Code</param>
        /// 
        public List<LocationUniqueFormattedName> LocationUniqueFormattedName(string countryCode, string languageCode)
        {
            var query = string.Format("SELECT [Id] AS ID,[CountryCode] AS COUNTRYCODE,[IndexLanguage] AS INDEXLANGUAGE,[Name] AS NAME FROM [Platform].[dbo].[LocationUniqueFormattedNames] where CountryCode = '{0}' and IndexLanguage = '{1}'", countryCode, languageCode);
            var uniqueNames = new List<LocationUniqueFormattedName>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["INDEXLANGUAGE"] != DBNull.Value && reader["NAME"] != DBNull.Value)
                            {
                                uniqueNames.Add(new LocationUniqueFormattedName
                                {
                                    Id = (long)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    IndexLanguage = reader["INDEXLANGUAGE"].ToString(),
                                    Name = reader["NAME"].ToString()
                                });
                            }
                        }
                        reader.Close();
                    }
                }
                return uniqueNames;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// <param name="languageCodes">Language Codes</param>
        /// <param name="languageCode">Language Code</param>
        /// <returns></returns>
        public List<Tuple<string,object>> GetAliasesAndUniqueNames(string countryCode, List<string> languageCodes, string languageCode)
        {
            var resultSets = new List<Tuple<string, object>>();
            var locationAliases = new List<LocationAlias>();
            var uniqueNames = new List<LocationUniqueFormattedName>();

            var query = string.Format("SELECT [Id] AS ID,[CountryCode] AS COUNTRYCODE,[IndexLanguage] AS INDEXLANGUAGE,[Name] AS NAME FROM [Platform].[dbo].[LocationUniqueFormattedNames] where CountryCode = '{0}' and IndexLanguage = '{1}';", countryCode, languageCode);

            var selectStatement = "SELECT [Platform].[dbo].[LocationAlias].[SOURCEID] AS SOURCEID " +
                                ",[Platform].[dbo].[LocationAlias].[NAME] AS NAME " +
                                ",[LANGUAGECODE] AS LANGUAGECODE " +
                                ",[ISPREFERREDNAME] AS ISPREFERREDNAME " +
                                ",[ISSHORTNAME] AS ISSHORTNAME " +
                                "FROM [Platform].[dbo].[LocationAlias] " +
                                "INNER JOIN [Platform].[dbo].[Location] " +
                                "ON [Platform].[dbo].[LocationAlias].[SOURCEID] = [Platform].[dbo].[Location].[ID] ";

            var whereCountryClause = string.Format("where [Platform].[dbo].[Location].[CountryCode] = '{0}' ", countryCode);

            var whereLanguageCodeClause = new StringBuilder();
            if (languageCodes.Any())
            {
                var isFirst = true;
                whereLanguageCodeClause.Append(" and (");
                foreach (var lanCode in languageCodes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        whereLanguageCodeClause.Append(" OR ");
                    if (languageCode.ToLower().Equals("null"))
                        whereLanguageCodeClause.Append(string.Format("[Platform].[dbo].[LocationAlias].[LanguageCode] is null"));
                    else
                        whereLanguageCodeClause.Append(string.Format("[Platform].[dbo].[LocationAlias].[LanguageCode] = '{0}'", lanCode));
                }
                whereLanguageCodeClause.Append(")");
            }

            query = query + selectStatement + whereCountryClause + whereLanguageCodeClause.ToString();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["INDEXLANGUAGE"] != DBNull.Value && reader["NAME"] != DBNull.Value)
                            {
                                uniqueNames.Add(new LocationUniqueFormattedName
                                {
                                    Id = (long)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    IndexLanguage = reader["INDEXLANGUAGE"].ToString(),
                                    Name = reader["NAME"].ToString()
                                });
                            }
                        }
                        resultSets.Add(new Tuple<string, object>("alias", locationAliases));

                        reader.NextResult();

                        while (reader.Read())
                        {
                            if (reader["SOURCEID"] != DBNull.Value && reader["NAME"] != DBNull.Value && reader["LANGUAGECODE"] != DBNull.Value && reader["ISPREFERREDNAME"] != DBNull.Value && reader["ISSHORTNAME"] != DBNull.Value)
                            {
                                locationAliases.Add(new LocationAlias
                                {
                                    SourceId = (long)reader["SOURCEID"],
                                    Name = reader["NAME"].ToString(),
                                    LanguageCode = reader["LANGUAGECODE"].ToString(),
                                    IsPreferredName = (bool)reader["ISPREFERREDNAME"],
                                    IsShortName = (bool)reader["ISSHORTNAME"]
                                });
                            }
                        }
                        resultSets.Add(new Tuple<string, object>("uniqueformat", uniqueNames));
                        reader.Close();
                    }
                }
                var aliases = locationAliases.Where(x => !x.Name.ContainsDigits()).ToList();

                return resultSets;
            }
        }

        /// <summary>
        /// Get multiple resultsets.
        /// </summary>
        /// 
        public List<Tuple<string,object>> GetMultiResultSets()
        {
            var query = "SELECT [ID],[COUNTRYCODE],[CHARACTER],[REPLACEMENT] FROM [Platform].[dbo].[LocationCharacterTransformations] ORDER BY [COUNTRYCODE]; " +
            "SELECT [ID],[NAME],[REGEXSTRING],[ABBREVIATIONS] FROM [Platform].[dbo].[LocationAbbreviation]; " +
            "SELECT [ID],[COUNTRYCODE],[SOURCEID],[ACTION] FROM [Platform].[dbo].[LocationCleanserDataDirective]; " +
            "SELECT [ID],[COUNTRYCODE],[LANGUAGECODE],[SOURCEID],[NAME] FROM [platform].[dbo].[LocationNames]; " +
            "SELECT [COUNTRYCODE] FROM [Platform].[dbo].[Location] WHERE TYPEID = 0 ORDER BY COUNTRYCODE; " +
            "SELECT [ID],[SOURCEID],[COUNTRYCODE],[NAME] FROM [platform].[dbo].[LocationSuggestionInputs] ORDER BY [SOURCEID]; " +
            "SELECT [SOURCEID],[COUNTRYCODE],[LANGUAGECODE],[USECUSTOMNAME],[USEPREFERREDNAME],[USESHORTNAME]FROM [Platform].[dbo].[PreferredLocationNameSelections] ORDER BY [COUNTRYCODE], [LANGUAGECODE], [SOURCEID]";

            var transformations = new List<CharacterTransformation>();
            var abbreviations = new List<LocationAbbreviation>();
            var directives = new List<LocationCleanserDirective>();
            var names = new List<LocationName>();
            var countryCodes = new List<string>();
            var suggestions = new List<LocationSuggestionInput>();
            var selections = new List<PreferredLocationNameSelection>();

            var resultSets = new List<Tuple<string, object>>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 5000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        // Character Transformations
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["CHARACTER"] != DBNull.Value && reader["REPLACEMENT"] != DBNull.Value)
                            {
                                transformations.Add(new CharacterTransformation
                                {
                                    Id = (int)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    Character = reader["CHARACTER"].ToString(),
                                    Replacement = reader["REPLACEMENT"].ToString()
                                });
                            }
                        }
                        resultSets.Add(new Tuple<string, object>("charactertransformation", transformations));

                        reader.NextResult();

                        // Location Abbreviations
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["NAME"] != DBNull.Value && reader["REGEXSTRING"] != DBNull.Value && reader["ABBREVIATIONS"] != DBNull.Value)
                            {
                                abbreviations.Add(new LocationAbbreviation
                                {
                                    Id = (int)reader["ID"],
                                    Name = reader["NAME"].ToString(),
                                    RegexString = reader["REGEXSTRING"].ToString(),
                                    Abbreviations = reader["ABBREVIATIONS"].ToString()
                                });
                            }
                        }
                        resultSets.Add(new Tuple<string, object>("locationabbreviation", abbreviations));

                        reader.NextResult();

                        // Location Cleanser Directives
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["SOURCEID"] != DBNull.Value && reader["ACTION"] != DBNull.Value)
                            {
                                directives.Add(new LocationCleanserDirective
                                {
                                    Id = (long)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    SourceId = (long)reader["SOURCEID"],
                                    Action = (int)reader["ACTION"]
                                });
                            }
                        }
                        resultSets.Add(new Tuple<string, object>("locationcleanserdirective", directives));

                        reader.NextResult();

                        // Location Names
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["SOURCEID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["NAME"] != DBNull.Value && reader["LANGUAGECODE"] != DBNull.Value)
                            {
                                names.Add(new LocationName
                                {
                                    ID = (long)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    LanguageCode = reader["LANGUAGECODE"].ToString(),
                                    SourceID = (long)reader["SOURCEID"],
                                    Name = reader["NAME"].ToString()
                                });
                            }
                        }
                        resultSets.Add(new Tuple<string, object>("locationname", names));

                        reader.NextResult();

                        // Country Codes
                        while (reader.Read())
                        {
                            if (reader["COUNTRYCODE"] != DBNull.Value)
                                countryCodes.Add(reader["COUNTRYCODE"].ToString().ToUpper());
                        }
                        resultSets.Add(new Tuple<string, object>("countrycode", countryCodes));

                        reader.NextResult();

                        // Location Suggestion Inputs
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["SOURCEID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["NAME"] != DBNull.Value)
                            {
                                suggestions.Add(new LocationSuggestionInput
                                {
                                    Id = (int)reader["ID"],
                                    SourceId = (long)reader["SOURCEID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    Name = reader["NAME"].ToString()
                                });
                            }
                        }
                        resultSets.Add(new Tuple<string, object>("locationsuggestioninput", suggestions));

                        reader.NextResult();

                        // Preferred Location Name Selection
                        while (reader.Read())
                        {
                            if (reader["SOURCEID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["LANGUAGECODE"] != DBNull.Value && reader["USECUSTOMNAME"] != DBNull.Value && reader["USEPREFERREDNAME"] != DBNull.Value && reader["USESHORTNAME"] != DBNull.Value)
                            {
                                selections.Add(new PreferredLocationNameSelection
                                {
                                    SourceID = (long)reader["SOURCEID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    LanguageCode = reader["LANGUAGECODE"].ToString(),
                                    UseCustomName = (bool)reader["USECUSTOMNAME"],
                                    UsePreferredName = (bool)reader["USEPREFERREDNAME"],
                                    UseShortName = (bool)reader["USESHORTNAME"]
                                });
                            }
                        }
                        resultSets.Add(new Tuple<string, object>("preferredlocationnameselection", selections));
                        reader.Close();
                    }
                }
            }
            return resultSets;
        }

        /// <summary>
        /// Gets the Characters used to add suggestions for certain locations.
        /// For example: 'söc' would have a suggestion.input 'soec'.
        /// </summary>
        /// 
        public List<CharacterTransformation> GetCharacterTransformations()
        {
            var query = string.Format("SELECT [ID],[COUNTRYCODE],[CHARACTER],[REPLACEMENT] FROM [Platform].[dbo].[LocationCharacterTransformations] ORDER BY [COUNTRYCODE]");
            var abbreviations = new List<CharacterTransformation>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["CHARACTER"] != DBNull.Value && reader["REPLACEMENT"] != DBNull.Value)
                            {
                                abbreviations.Add(new CharacterTransformation
                                {
                                    Id = (int)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    Character = reader["CHARACTER"].ToString(),
                                    Replacement = reader["REPLACEMENT"].ToString()
                                });
                            }
                        }
                        reader.Close();
                    }
                }

                return abbreviations;
            }
        }

        /// <summary>
        /// Get the list of countries codes.
        /// </summary>
        /// 
        public List<string> GetCountryCodes()
        {
            var countryCodes = new List<string>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = "SELECT [COUNTRYCODE] FROM [Platform].[dbo].[Location] WHERE TYPEID = 0 ORDER BY COUNTRYCODE";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["COUNTRYCODE"] != DBNull.Value)
                                countryCodes.Add(reader["COUNTRYCODE"].ToString().ToUpper());
                        }
                        reader.Close();
                    }
                }
            }
            return countryCodes;
        }

        /// <summary>
        /// Returns the feature codes for the submitted country codes
        /// </summary>
        /// <returns>List of Tuples(CountryCode,FeatureCode)</returns>
        /// 
        public List<Tuple<string, string>> GetFeatureCodes()
        {
            var results = new List<Tuple<string, string>>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = "SELECT [COUNTRYCODE], [FEATURECODE] FROM [Platform].[dbo].[CountryFeatureCodes] ORDER BY COUNTRYCODE;";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["COUNTRYCODE"] != DBNull.Value && reader["FEATURECODE"] != DBNull.Value)
                                results.Add(new Tuple<string, string>(reader["COUNTRYCODE"].ToString().ToUpper(), reader["FEATURECODE"].ToString().ToUpper()));
                        }
                        reader.Close();
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Returns the abbreviations for locations. Example: Fort to Ft, Saint to St.
        /// </summary>
        /// 
        public List<LocationAbbreviation> GetLocationAbbreviations()
        {
            var query = string.Format("SELECT [ID],[NAME],[REGEXSTRING],[ABBREVIATIONS] FROM [Platform].[dbo].[LocationAbbreviation]");
            var abbreviations = new List<LocationAbbreviation>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["NAME"] != DBNull.Value && reader["REGEXSTRING"] != DBNull.Value && reader["ABBREVIATIONS"] != DBNull.Value)
                            {
                                abbreviations.Add(new LocationAbbreviation
                                {
                                    Id = (int)reader["ID"],
                                    Name = reader["NAME"].ToString(),
                                    RegexString = reader["REGEXSTRING"].ToString(),
                                    Abbreviations = reader["ABBREVIATIONS"].ToString()
                                });
                            }
                        }
                        reader.Close();
                    }
                }

                return abbreviations;
            }
        }

        /// <summary>
        /// Returns a list of LocationCleanserDirective objects. 
        /// These objects indicate locations that are to be deleted or not deleted from the index.
        /// </summary>
        /// 
        public List<LocationCleanserDirective> GetLocationCleanserDirectives()
        {
            var query = string.Format("SELECT [ID],[COUNTRYCODE],[SOURCEID],[ACTION] FROM [Platform].[dbo].[LocationCleanserDataDirective]");
            return PerformQuery(query);
        }

        /// <summary>
        /// Returns a list of LocationCleanserDirective objects. 
        /// These objects indicate locations that are to be deleted or not deleted from the index.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// 
        public List<LocationCleanserDirective> GetLocationCleanserDirectives(string countryCode)
        {
            var query = string.Format("SELECT [ID],[COUNTRYCODE],[SOURCEID],[ACTION] FROM [Platform].[dbo].[LocationCleanserDataDirective] WHERE COUNTRYCODE = {0}", countryCode.ToUpper());
            return PerformQuery(query);
        }

        /// <summary>
        /// Get the custom names for a location. This data is used in situations where it is not possible to programmatically
        /// obtain the preferred name for a location.
        /// </summary>
        /// 
        public List<LocationName> GetLocationNames()
        {
            var query = string.Format("SELECT [ID],[COUNTRYCODE],[LANGUAGECODE],[SOURCEID],[NAME] FROM [platform].[dbo].[LocationNames]");
            var names = new List<LocationName>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["SOURCEID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["NAME"] != DBNull.Value && reader["LANGUAGECODE"] != DBNull.Value)
                            {
                                names.Add(new LocationName
                                {
                                    ID = (long)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    LanguageCode = reader["LANGUAGECODE"].ToString(),
                                    SourceID = (long)reader["SOURCEID"],
                                    Name = reader["NAME"].ToString()
                                });
                            }
                        }
                        reader.Close();
                    }
                }
                return names;
            }
        }

        /// <summary>
        /// Returns the suggestions for locations. Alternate names that are used locally but not found in GeoNames dataset. 
        /// Example: Local names for military bases.
        /// </summary>
        /// 
        public List<LocationSuggestionInput> GetLocationSuggestionInputs()
        {
            var query = string.Format("SELECT [ID],[SOURCEID],[COUNTRYCODE],[NAME] FROM [platform].[dbo].[LocationSuggestionInputs] ORDER BY [SOURCEID]");
            var suggestions = new List<LocationSuggestionInput>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["SOURCEID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["NAME"] != DBNull.Value)
                            {
                                suggestions.Add(new LocationSuggestionInput
                                {
                                    Id = (int)reader["ID"],
                                    SourceId = (long)reader["SOURCEID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    Name = reader["NAME"].ToString()
                                });
                            }
                        }
                        reader.Close();
                    }
                }

                return suggestions;
            }
        }

        /// <summary>
        /// Returns the preferred names directives for locations. These objects indicate where to find the preferred name for a specified location.
        /// </summary>
        /// <param name="countryCode">Country Code</param>
        /// 
        public List<PreferredLocationNameSelection> GetPreferredLocationNameSelections(string countryCode = null)
        {
            var query = string.IsNullOrWhiteSpace(countryCode)
                    ? string.Format("SELECT [SOURCEID],[COUNTRYCODE],[LANGUAGECODE],[USECUSTOMNAME],[USEPREFERREDNAME],[USESHORTNAME]FROM [Platform].[dbo].[PreferredLocationNameSelections] ORDER BY [COUNTRYCODE], [LANGUAGECODE], [SOURCEID]")
                    : string.Format("SELECT [SOURCEID],[COUNTRYCODE],[LANGUAGECODE],[USECUSTOMNAME],[USEPREFERREDNAME],[USESHORTNAME]FROM [Platform].[dbo].[PreferredLocationNameSelections] WHERE [COUNTRYCODE] = '{0}' ORDER BY [COUNTRYCODE], [LANGUAGECODE], [SOURCEID]", countryCode);

            var selections = new List<PreferredLocationNameSelection>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["SOURCEID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["LANGUAGECODE"] != DBNull.Value && reader["USECUSTOMNAME"] != DBNull.Value && reader["USEPREFERREDNAME"] != DBNull.Value && reader["USESHORTNAME"] != DBNull.Value)
                            {
                                selections.Add(new PreferredLocationNameSelection
                                {
                                    SourceID = (long)reader["SOURCEID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    LanguageCode = reader["LANGUAGECODE"].ToString(),
                                    UseCustomName = (bool)reader["USECUSTOMNAME"],
                                    UsePreferredName = (bool)reader["USEPREFERREDNAME"],
                                    UseShortName = (bool)reader["USESHORTNAME"]
                                });
                            }
                        }
                        reader.Close();
                    }
                }

                return selections;
            }
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns a list of LocationCleanserDirective objects. 
        /// These objects indicate locations that are to be deleted or not deleted from the index.
        /// </summary>
        /// <param name="query">SQL quert string</param>
        /// 
        private List<LocationCleanserDirective> PerformQuery(string query)
        {
            var directives = new List<LocationCleanserDirective>();

            using (var conn = new SqlConnection(_sqlPlatformConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["COUNTRYCODE"] != DBNull.Value && reader["SOURCEID"] != DBNull.Value && reader["ACTION"] != DBNull.Value)
                            {
                                directives.Add(new LocationCleanserDirective
                                {
                                    Id = (long)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    SourceId = (long)reader["SOURCEID"],
                                    Action = (int)reader["ACTION"]
                                });
                            }
                        }
                        reader.Close();
                    }
                }

                return directives;
            }
        }
        #endregion Private Region
    }
}
