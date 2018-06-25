using Domain.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace DataAccess
{
    public class AdoClient
    {
        ConnectionStringSettings _sqlConnectionString = null;

        public AdoClient()
        {
            _sqlConnectionString = ConfigurationManager.ConnectionStrings["PlaformSqlConnectionString"];
        }

        public List<LocationView> GetLocations(string query)
        {
            var geoLocations = new List<LocationView>();

            using (var conn = new SqlConnection(_sqlConnectionString.ToString()))
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
    }
}
