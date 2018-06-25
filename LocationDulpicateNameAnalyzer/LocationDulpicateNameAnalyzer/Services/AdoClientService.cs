using FastMember;
using LocationDulpicateNameAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LocationDulpicateNameAnalyzer.Services
{
    public class AdoClientService
    {
        ConnectionStringSettings _sqlConnectionString = null;
        ConnectionStringSettings _extDataConnectionString = null;

        public AdoClientService()
        {
            _sqlConnectionString = ConfigurationManager.ConnectionStrings["PlaformSqlConnectionString"];
            _extDataConnectionString = ConfigurationManager.ConnectionStrings["GeonamesConnString-local"];
        }

        public List<long> GetLocationIds(string countryCode)
        {
            var ids = new List<long>();

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = string.Format("SELECT [ID] FROM [ExternalData].[GeoNames].[Location] WHERE CountryCode = '{0}' AND IsIndexed = 1",countryCode.ToLower());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value)
                                ids.Add((long) reader["ID"]);
                        }
                        reader.Close();
                    }
                }
            }
            return ids;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <param name="table"></param>
        public void SetLocationFormattedNames(List<LocationFormattedName> names, DataTable table)
        {
            using (var reader = ObjectReader.Create(names))
            {
                table.Load(reader);
            }

            using (var conn = new SqlConnection(_sqlConnectionString.ToString()))
            {
                conn.Open();

                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                {
                    bulkcopy.DestinationTableName = "dbo.LocationFormattedNames";

                    try
                    {
                        bulkcopy.WriteToServer(table);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                conn.Close();
            }
        }

        public void SetDuplicateLocations(List<LocationUniqueFormattedName> names, DataTable table)
        {
            using (var reader = ObjectReader.Create(names))
            {
                table.Load(reader);
            }

            using (var conn = new SqlConnection(_sqlConnectionString.ToString()))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();
 
                        command.CommandText = "IF OBJECT_ID('tempdb..#Staging_Locations_Unique_Names') IS NOT NULL DROP TABLE #Staging_Locations_Unique_Names";
                        command.ExecuteNonQuery();

                        var createTmpTable = "CREATE TABLE #Staging_Locations_Unique_Names (" +
                                        " ID bigint, " +
                                        " CountryCode nvarchar(4), " +
                                        " IndexLanguage nvarchar(4), " +
                                        " FormattedName nvarchar(max), " +
                                        " Name nvarchar(max))";
                        command.CommandText = createTmpTable;
                        command.ExecuteNonQuery();

                        // dbo.LocationUniqueFormattedNames
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.DestinationTableName = "#Staging_Locations_Unique_Names";
                            bulkcopy.WriteToServer(table);
                            bulkcopy.Close();

                           command.CommandTimeout = 5000;
                           command.CommandText = "UPDATE LocationUniqueFormattedNames SET LocationUniqueFormattedNames.Name = #Staging_Locations_Unique_Names.Name FROM LocationUniqueFormattedNames INNER JOIN #Staging_Locations_Unique_Names ON LocationUniqueFormattedNames.Id = #Staging_Locations_Unique_Names.Id and LocationUniqueFormattedNames.CountryCode = #Staging_Locations_Unique_Names.CountryCode and LocationUniqueFormattedNames.IndexLanguage = #Staging_Locations_Unique_Names.IndexLanguage; DROP TABLE #Staging_Locations_Unique_Names;";
                           command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        // Cleanup
                        conn.Close();
                    }
                }
            }
        }

        public void FormattedNameForAnalysis(List<LocationFormattedNamesAnalysis> formattedNames, DataTable table)
        {
            using (var reader = ObjectReader.Create(formattedNames))
            {
                table.Load(reader);
            }

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                conn.Open();

                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                {
                    bulkcopy.DestinationTableName = "GeoNames.LocationFormattedNamesAnalysis";

                    try
                    {
                        bulkcopy.WriteToServer(table);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                conn.Close();
            }
        }

       

        public void FormattedNameForAnalysis(LocationFormattedNamesAnalysis formattedName)
        {
            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();
                        command.CommandTimeout = 5000;
                        command.CommandText = string.Format("INSERT INTO [GeoNames].[LocationFormattedNamesAnalysis]([ID],[CountryCode],[LanguageCode],[FormattedName])VALUES({0},{1},{2},{3})", formattedName.ID, formattedName.CountryCode, formattedName.LanguageCode, formattedName.FormattedName);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public List<LocationHierarchyAnalysis> GetDuplicatesWithoutMappedToId(string countryCode)
        {
            var analysis = new List<LocationHierarchyAnalysis>();

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = string.Format("SELECT [ID],[POPULATION],[PNAME],[COUNTRYCODE],[MAPPEDTOID] FROM [ExternalData].[GeoNames].[LocationHierarchyAnalysisBk] WHERE COUNTRYCODE = '{0}' and MAPPEDTOID is null ORDER BY PNAME", countryCode);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["POPULATION"] != DBNull.Value && reader["PNAME"] != DBNull.Value)
                                analysis.Add(new LocationHierarchyAnalysis
                                {
                                    Id = (long)reader["ID"],
                                    Population = (long)reader["POPULATION"],
                                    PName = reader["PNAME"].ToString(),
                                    CountryCode = reader["COUNTRYCODE"].ToString()
                                });
                        }
                        reader.Close();
                    }
                }
            }
            return analysis;
        }

        public LocationHierarchyAnalysis GetIndexedHierarchyAnalysis(string countryCode, string pName)
        {
            var analysis = new LocationHierarchyAnalysis();

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = string.Format("SELECT [ID],[POPULATION],[PNAME],[COUNTRYCODE],[MAPPEDTOID] FROM [ExternalData].[GeoNames].[LocationHierarchyAnalysisBk] WHERE COUNTRYCODE = '{0}' and PNAME = N'{1}' and MAPPEDTOID is not null", countryCode, pName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && reader["POPULATION"] != DBNull.Value && reader["PNAME"] != DBNull.Value && reader["MAPPEDTOID"] != DBNull.Value)
                                analysis = new LocationHierarchyAnalysis
                                {
                                    Id = (long)reader["ID"],
                                    Population = (long)reader["POPULATION"],
                                    PName = reader["PNAME"].ToString(),
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    MappedToId = (long) reader["MAPPEDTOID"]
                                };
                        }
                        reader.Close();
                    }
                }
            }
            return analysis;
        }

        public void UpdateHierarchyAnalysis(long id, long mappedToId)
        {
            var query = string.Format("UPDATE [GeoNames].[LocationHierarchyAnalysisBk] SET [MappedToId] = {0} WHERE ID = {1}", mappedToId, id );

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();
                        command.CommandTimeout = 5000;
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("ID: {0}, MAPPED TO ID: {1}, MESSAGE: {2}", id, mappedToId, ex.Message));
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public List<string> GetDuplicateFormattedNames(string countryCode)
        {
            var analysis = new List<string>();

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = string.Format("SELECT [FormattedNameNon] FROM (SELECT [FormattedNameNon], Count(1) as cnt FROM [ExternalData].[GeoNames].[LocationFormattedNamesAnalysis] WHERE CountryCode = '{0}' GROUP BY [FormattedNameNon]) as distinctNames WHERE cnt > 1", countryCode);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["FORMATTEDNAMENON"] != DBNull.Value)
                                analysis.Add(reader["FORMATTEDNAMENON"].ToString());
                        }
                        reader.Close();
                    }
                }
            }
            return analysis;
        }

        public List<LocationFormattedNamesAnalysis> GetDuplicates(List<string> duplicateNames, string countryCode)
        {
            var queryStr = new StringBuilder();
            queryStr.Append(string.Format("SELECT [ID],[COUNTRYCODE],[LANGUAGECODE],[FORMATTEDNAME],[FORMATTEDNAMENON],[TYPEID],[FORMATTING] FROM [ExternalData].[GeoNames].[LocationFormattedNamesAnalysis] WHERE [COUNTRYCODE] = '{0}' AND (", countryCode));

            var isFirst = true;
            foreach (var duplicateName in duplicateNames)
            {
                if (isFirst)
                    isFirst = false;
                else
                    queryStr.Append(" OR ");
                if (duplicateName.Contains("'"))
                {
                    var name = duplicateName.Replace("'", "''");
                    queryStr.Append(string.Format("FormattedNameNon = '{0}'", name));
                }
                else
                    queryStr.Append(string.Format("FormattedNameNon = '{0}'", duplicateName));


            }
            queryStr.Append(")");

            var result = queryStr.ToString();
            var analysis = new List<LocationFormattedNamesAnalysis>();

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = queryStr.ToString();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value &&
                                reader["COUNTRYCODE"] != DBNull.Value &&
                                reader["LANGUAGECODE"] != DBNull.Value &&
                                reader["FORMATTEDNAME"] != DBNull.Value &&
                                reader["FORMATTEDNAMENON"] != DBNull.Value &&
                                reader["TYPEID"] != DBNull.Value &&
                                reader["FORMATTING"] != DBNull.Value
                            )
                                analysis.Add(new LocationFormattedNamesAnalysis {
                                    ID = (long)reader["ID"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    LanguageCode = reader["LANGUAGECODE"].ToString(),
                                    FormattedName = reader["FORMATTEDNAME"].ToString(),
                                    FormattedNameNon = reader["FORMATTEDNAMENON"].ToString(),
                                    TypeId = (int)reader["TYPEID"],
                                    Formatting = (int)reader["FORMATTING"]
                                });
                        }
                        reader.Close();
                    }
                }
            }
            return analysis;
        }

        public int GetNumberOfTypes(string formattedName, string countryCode)
        {
            var total = 0;

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = string.Format("SELECT TOP 1 COUNT(*) OVER () AS TOTALRECORDS FROM [ExternalData].[GeoNames].[LocationFormattedNamesAnalysis] WHERE FormattedNameNon = '{0}' and CountryCode = '{1}' GROUP BY [ExternalData].[GeoNames].[LocationFormattedNamesAnalysis].[TypeId]", formattedName, countryCode);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["TOTALRECORDS"] != DBNull.Value)
                               total = (int) reader["TOTALRECORDS"];
                        }
                        reader.Close();
                    }
                }
            }
            return total;
        }

        public int GetNumberOfRows(string formattedName, string countryCode)
        {
            var numberOfRows = 0;

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = string.Format("COUNT(*) AS NUMBEROFROWS FROM[ExternalData].[GeoNames].[LocationFormattedNamesAnalysis] WHERE FormattedNameNon = '{0}' AND CountryCode =  '{1}'", formattedName, countryCode);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["NUMBEROFROWS"] != DBNull.Value)
                                numberOfRows = (int)reader["NUMBEROFROWS"];
                        }
                        reader.Close();
                    }
                }
            }
            return numberOfRows;
        }

        public void SetFormatting(List<LocationFormattedNamesAnalysis> names, int formatCode)
        {
            var idList = new StringBuilder();
            var isFirst = true;
            foreach (var name in names)
            {
                if (isFirst)
                    isFirst = false;
                else
                    idList.Append(", ");
                idList.Append(name.ID);
            }
           
            var query = string.Format("UPDATE [ExternalData].[GeoNames].[LocationFormattedNamesAnalysis] SET[ExternalData].[GeoNames].[LocationFormattedNamesAnalysis].[Formatting] = {0} WHERE ID IN({1})", formatCode, idList);

            using (var conn = new SqlConnection(_extDataConnectionString.ToString()))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();
                        command.CommandTimeout = 5000;
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public List<LocationFormattedNamesAnalysis> GetNamesAnalysis(string countryCode)
        {
            var analysis = new List<LocationFormattedNamesAnalysis>();

            using (var conn = new SqlConnection(_sqlConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = string.Format("SELECT [ID],[COUNTRYCODE],[LANGUAGECODE],[FORMATTEDNAME],[FORMATTEDNAMENON],[TYPEID],[FORMATTING] FROM [ExternalData].[GeoNames].[LocationFormattedNamesAnalysis] WHERE [COUNTRYCODE] = {0}", countryCode);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value &&
                                reader["COUNTRYCODE"] != DBNull.Value &&
                                reader["LANGUAGECODE"] != DBNull.Value &&
                                reader["FORMATTEDNAME"] != DBNull.Value &&
                                reader["FORMATTEDNAMENON"] != DBNull.Value &&
                                reader["TYPEID"] != DBNull.Value &&
                                reader["FORMATTING"] != DBNull.Value
                            )
                                analysis.Add(new LocationFormattedNamesAnalysis
                                {
                                    ID = (long)reader["FORMATTEDNAMENON"],
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    LanguageCode = reader["LANGUAGECODE"].ToString(),
                                    FormattedName = reader["FORMATTEDNAME"].ToString(),
                                    FormattedNameNon = reader["FORMATTEDNAMENON"].ToString(),
                                    TypeId = (int)reader["TYPEID"],
                                    Formatting = (int)reader["FORMATTING"]
                                });
                               
                        }
                        reader.Close();
                    }
                }
            }
            return analysis;
        }

        public List<LocationUniqueFormattedName> GetDuplicateLocations(string languageCode, string countryCode = "all")
        {
            var query = new StringBuilder();

            query.Append("SELECT [ID],[COUNTRYCODE],[INDEXLANGUAGE],[FORMATTEDNAME],[NAME] FROM [Platform].[dbo].[LocationUniqueFormattedNames]");
            if (countryCode == "all")
                query.Append(string.Format(" WHERE INDEXLANGUAGE = '{0}'", languageCode));
            else
                query.Append(string.Format(" WHERE INDEXLANGUAGE = '{0}' AND COUNTRYCODE = '{1}'", languageCode, countryCode));

            var uniqueNames = new List<LocationUniqueFormattedName>();
            using (var conn = new SqlConnection(_sqlConnectionString.ToString()))
            {
                using (var command = conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandTimeout = 4000;
                    command.CommandText = query.ToString();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["ID"] != DBNull.Value && 
                                reader["COUNTRYCODE"] != DBNull.Value && 
                                reader["INDEXLANGUAGE"] != DBNull.Value && 
                                reader["FORMATTEDNAME"] != DBNull.Value 
                                )
                            {
                                uniqueNames.Add(new LocationUniqueFormattedName
                                {
                                    Id = (long)reader["ID"],
                                    Name = (reader["NAME"] != DBNull.Value) ? reader["NAME"].ToString() : string.Empty,
                                    CountryCode = reader["COUNTRYCODE"].ToString(),
                                    FormattedName = reader["FORMATTEDNAME"].ToString(),
                                    IndexLanguage = reader["INDEXLANGUAGE"].ToString()
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
        /// Get the list of countries codes.
        /// </summary>
        /// 
        public List<string> GetCountryCodes()
        {
            var countryCodes = new List<string>();

            using (var conn = new SqlConnection(_sqlConnectionString.ToString()))
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
    }
}
