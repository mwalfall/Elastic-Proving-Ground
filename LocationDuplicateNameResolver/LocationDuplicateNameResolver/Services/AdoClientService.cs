using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using FastMember;
using LocationDuplicateNameResolver.Model;

namespace LocationDuplicateNameResolver.Services
{
    public class AdoClientService
    {
        ConnectionStringSettings _sqlConnectionString = null;

        public AdoClientService()
        {
            _sqlConnectionString = ConfigurationManager.ConnectionStrings["PlaformSqlConnectionString"];
        }

        public void SetLocationFormattedNames(List<LocationFormattedName> names)
        {
            DataTable table = new DataTable();
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
    }
}
