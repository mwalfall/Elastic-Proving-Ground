using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationDulpicateNameAnalyzer.Services
{
    public static class DataTableBuilder
    {
        public static DataTable Get()
        {
            DataTable table = new DataTable("LocationFormattedNames");
            DataColumn id = table.Columns.Add("Id", typeof(Int64));
            id.AllowDBNull = false;
            table.Columns.Add("CountryCode", typeof(string));
            table.Columns.Add("IndexLanguage", typeof(string));
            table.Columns.Add("FormattedName", typeof(string));

            return table;
        }

        public static DataTable GetLocationUniqueFormattedNameTable()
        {
            DataTable table = new DataTable("LocationUniqueFormattedNames");
            DataColumn id = table.Columns.Add("Id", typeof(Int64));
            id.AllowDBNull = false;
            table.Columns.Add("CountryCode", typeof(string));
            table.Columns.Add("IndexLanguage", typeof(string));
            table.Columns.Add("FormattedName", typeof(string));
            table.Columns.Add("Name", typeof(string));

            return table;
        }

        public static DataTable GetFormattedNamesAnalysisTable()
        {
            DataTable table = new DataTable("LocationFormattedNamesAnalysis");
            DataColumn id = table.Columns.Add("Id", typeof(Int64));
            id.AllowDBNull = false;
            table.Columns.Add("CountryCode", typeof(string));
            table.Columns.Add("LanguageCode", typeof(string));
            table.Columns.Add("FormattedName", typeof(string));
            table.Columns.Add("FormattedNameNon", typeof(string));
            table.Columns.Add("TypeId", typeof(Int32));
            table.Columns.Add("Formatting", typeof(Int32));

            return table;
        }
    }
}
