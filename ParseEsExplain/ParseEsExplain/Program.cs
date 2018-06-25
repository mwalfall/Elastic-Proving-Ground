using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace ParseEsExplain
{
    class Program
    {
        private Dictionary<string, int> _titlePosition = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            String JSONString = File.ReadAllText("EsExplain.json");
            var length = JSONString.Length;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = 90000000;

            dynamic jsonTransformation = ser.Deserialize<dynamic>(JSONString);

            var queryResults = GetHits(jsonTransformation);

            var queryScoreResults = new List<QueryScoreResult>();

            foreach(var queryResult in queryResults)
            {
                queryScoreResults.Add(ProcessQueryResult(queryResult));
            }

            CreateSpreadsheet(queryScoreResults);
        }

        private static void CreateSpreadsheet(List<QueryScoreResult> queryResults)
        {
            var titlePosition = new Dictionary<string, int>();

            var oXL = new Application();
            oXL.Visible = true;
            var oWB = (_Workbook)(oXL.Workbooks.Add(""));
            var oSheet = (_Worksheet) oWB.ActiveSheet;

            var firstIteration = true;

            var row = 1;

            foreach(var result in queryResults)
            {
                // On first iteration build the header row.
                if (firstIteration)
                {                    
                    oSheet.Cells[row, 1] = "Title";

                    var col = 2;
                    foreach(var score in result.QueryScores)
                    {
                        var title = ParseDescription(score.Description);
                        oSheet.Cells[row, col] = title;
                        titlePosition.Add(title, col);
                        col++;                      
                    }

                    oSheet.Cells[row, 10] = "Sum";
                    oSheet.Cells[row, 11] = "Coords";
                    oSheet.Cells[row, 12] = "Score";

                    row++;

                    firstIteration = false;
                }

                oSheet.Cells[row, 1] = result.Title;
                oSheet.Cells[row, 10] = result.QueriesScore;
                if (result.Coord != null)
                    oSheet.Cells[row, 11] = string.Format("{0} : {1}", result.Coord.Value, result.Coord.Description);
                else
                    oSheet.Cells[row, 11] = "1.0";
                oSheet.Cells[row, 12] = result.FinalScore;

                foreach(var score in result.QueryScores)
                {
                    var col = GetColumnNumber(titlePosition, score.Description);
                    oSheet.Cells[row, col] = score.Score;
                }
                row++;
            }

            foreach(var title in titlePosition)
            {
                if (title.Value == 8)
                    oSheet.Cells[1, 8] = title.Key;

                if (title.Value == 9)
                    oSheet.Cells[1, 9] = title.Key;
            }

            oXL.Visible = false;
            oXL.UserControl = false;
            oWB.SaveAs("C:\\Temp\\QueryScoreResults.xlsx", XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, XlSaveAsAccessMode.xlNoChange,Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            oWB.Close();
        }

        private static int GetColumnNumber(Dictionary<string, int> titles, string description)
        {
            var title = ParseDescription(description);
            if (titles.ContainsKey(title))
                return titles[title];

            var nextTitleColumnNumber = titles.Count + 2;
            titles.Add(title, nextTitleColumnNumber);

            return nextTitleColumnNumber;
        }

        private static string ParseDescription(string description)
        {
            var startIndex = description.IndexOf('(') + 1;
            var endIndex = description.IndexOf(' ');
            if (endIndex == -1)
                return description;

            var title = description.Substring(startIndex, endIndex);
            endIndex = title.IndexOf(' ');
            title = title.Substring(0, endIndex); 
            return title;
        }

        private static object[] GetHits(Dictionary<string, object> jsonTransformation)
        {
            var hitSummary = (Dictionary<string, object>) jsonTransformation["hits"];
            var hits = (object[]) hitSummary["hits"];
            return hits;
        }

        private static QueryScoreResult ProcessQueryResult(Dictionary<string, object> obj)
        {
            var queryScoreResult = new QueryScoreResult();

            Dictionary<string, object> _source = (Dictionary<string, object>)obj["_source"];
            queryScoreResult.Title = (string)_source["title"];

            Dictionary<string, object> _explanation = (Dictionary<string, object>)obj["_explanation"];
            queryScoreResult.FinalScore = (decimal)_explanation["value"];

            dynamic details = _explanation["details"];

            // Query Scores
            if (details[0] != null)
            {
                var queryScores = new List<QueryScore>();

                var underScores = (Dictionary<string, object>)details[0];
                queryScoreResult.QueriesScore = (decimal)underScores["value"];

                var individualQueryScores = (object[])underScores["details"];
                foreach (var score in individualQueryScores)
                {
                    var queryScore = new QueryScore();
                    var item = (Dictionary<string, object>)score;
                    queryScore.Score = (decimal)item["value"];
                    var queryDescription = (string)item["description"];
                    if (queryDescription.ToLower().Equals("max of:"))
                    {
                        var individualQueryDetailsArray = (object[])item["details"];
                        var individualQueryDetails = (Dictionary<string, object>)individualQueryDetailsArray[0];
                        queryScore.Description = (string)individualQueryDetails["description"];
                    }
                    else
                        queryScore.Description = queryDescription;

                    queryScores.Add(queryScore);
                }

                queryScoreResult.QueryScores = queryScores;

            }

            // Coord score
            if (details.Length > 1)
            {
                var coords = details[1];
                var coord = new Coord
                {
                    Value = (decimal)coords["value"],
                    Description = (string)coords["description"]
                };
                queryScoreResult.Coord = coord;
            }

            return queryScoreResult;
        }
    }

    public class QueryScoreResult
    {
        public QueryScoreResult()
        {
            QueryScores = new List<QueryScore>();
        }

        public decimal FinalScore { get; set; }
        public decimal QueriesScore { get; set; }
        public Coord Coord { get; set; }
        public string Title { get; set; }
        public List<QueryScore> QueryScores { get; set; }
    }

    

    public class QueryScore
    {
       public decimal Score { get; set; }
       public string Description { get; set; }
    }

    public class Coord
    {
        public string Description { get; set; }
        public decimal Value { get; set; }
    }
}
