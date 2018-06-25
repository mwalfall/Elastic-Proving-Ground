using System;
using System.Data.SqlClient;

namespace Domain.Utilities
{
    public class GeoLocation
    {
        private long _id { get; set; }
        private string _name { get; set; }
        private double _latitude { get; set; }
        private double _longitude { get; set; }
        private string _countryCode { get; set; }
        private string _admin1Code { get; set; }
        private string _admin2Code { get; set; }
        private string _admin3Code { get; set; }
        private string _admin4Code { get; set; }
        private long _population { get; set; }

        
        public GeoLocation(SqlDataReader reader)
        {
            ProcessData(reader);
        }

        public long Id { get { return _id; } }
        public string Name { get { return _name; } }
        public double Latitude { get { return _latitude; } }
        public double Longitude { get { return _longitude; } }
        public string CountryCode { get { return _countryCode; } }
        public string Admin1Code { get { return _admin1Code; } }
        public string Admin2Code { get { return _admin2Code; } }
        public string Admin3Code { get { return _admin3Code; } }
        public string Admin4Code { get { return _admin4Code; } }
        public long Population { get { return _population; } }

        private void ProcessData(SqlDataReader reader)
        {
            if (reader["ID"] != DBNull.Value)
                _id = (long)reader["ID"];

            if (reader["NAME"] != DBNull.Value)
                _name = (string)reader["NAME"];

            if (reader["LATITUDE"] != DBNull.Value)
                _latitude = (double)reader["LATITUDE"];

            if (reader["LONGITUDE"] != DBNull.Value)
                _longitude = (double)reader["LONGITUDE"];

            if (reader["COUNTRYCODE"] != DBNull.Value)
                _countryCode = (string)reader["COUNTRYCODE"];

            if (reader["ADMIN1CODE"] != DBNull.Value)
                _admin1Code = (string)reader["ADMIN1CODE"];

            if (reader["ADMIN2CODE"] != DBNull.Value)
                _admin2Code = (string)reader["ADMIN2CODE"];

            if (reader["ADMIN3CODE"] != DBNull.Value)
                _admin3Code = (string)reader["ADMIN3CODE"];

            if (reader["ADMIN4CODE"] != DBNull.Value)
                _admin4Code = (string)reader["ADMIN4CODE"];

            if (reader["POPULATION"] != DBNull.Value)
                _population = (long)reader["POPULATION"];
        }
    }
}
