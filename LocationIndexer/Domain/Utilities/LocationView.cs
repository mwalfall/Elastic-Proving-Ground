using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Utilities
{
    public class LocationView
    {
        private long _id { get; set; }
        private string _name { get; set; }
        private string _code { get; set; }
        private string _countryCode { get; set; }
        private int _typeId { get; set; }
        private double _longitude { get; set; }
        private double _latitude { get; set; }
        private long _population { get; set; }
        private string _locationPath { get; set; }
        private string _parentLocationPath { get; set; }

        public LocationView(SqlDataReader reader)
        {
            ProcessData(reader);
        }

        public long Id { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Code { get { return _code; } set { _code = value; } }
        public string CountryCode { get { return _countryCode; } set { _countryCode = value; } }
        public int TypeId { get { return _typeId; } set { _typeId = value; } }
        public double Longitude { get { return _longitude; } set { _longitude = value; } }
        public double Latitude { get { return _latitude; } set { _latitude = value; } }
        public long Population { get { return _population; } set { _population = value; } }
        public string LocationPath { get { return _locationPath; } set { _locationPath = value; } }
        public string ParentLocationPath { get { return _parentLocationPath; } set { _parentLocationPath = value; } }

        private void ProcessData(SqlDataReader reader)
        {
            if (reader["ID"] != DBNull.Value)
                _id = (long)reader["ID"];

            if (reader["NAME"] != DBNull.Value)
                _name = (string)reader["NAME"];

            if (reader["CODE"] != DBNull.Value)
                _code = (string)reader["CODE"];

            if (reader["COUNTRYCODE"] != DBNull.Value)
                _countryCode = (string)reader["COUNTRYCODE"];

            if (reader["TYPEID"] != DBNull.Value)
                _typeId = (int)reader["TYPEID"];

            if (reader["LONGITUDE"] != DBNull.Value)
                _longitude = (double)reader["LONGITUDE"];

            if (reader["LATITUDE"] != DBNull.Value)
                _latitude = (double)reader["LATITUDE"];

            if (reader["POPULATION"] != DBNull.Value)
                _population = (long)reader["POPULATION"];

            if (reader["LOCATIONPATH"] != DBNull.Value)
                _locationPath = (string)reader["LOCATIONPATH"];

            if (reader["PARENTLOCATIONPATH"] != DBNull.Value)
                _parentLocationPath = (string)reader["PARENTLOCATIONPATH"]; 
        }
    }
}
