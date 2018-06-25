using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace uft8toUft16Converter
{

    class Program
    {
        static void Main(string[] args)
        {
            ProcessGeonamesData();
            //ProcessCanada();
        }

        private static void ProcessCanada()
        {
            var sr = new StreamReader(@"C:\Temp\GeoNamesData\Postal Codes\CAOriginalZipCodeset.txt");
            var outputFile = new StreamWriter(@"C:\Temp\GeoNamesData\Postal Codes\CA.txt", false, Encoding.Unicode);
            var cnt = 0;

            while (!sr.EndOfStream)
            {
                var strline = sr.ReadLine();

                if (strline == null)
                    continue;

                var values = strline.Split(',');
                var line = new List<string> { "CA", values[0].Replace('"', ' ').Trim(), values[3].Replace('"', ' ').Trim(), "", "", "", "", "", "", values[1].Replace('"', ' ').Trim(), values[2].Replace('"', ' ').Trim(), "" };
                outputFile.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}", line[0], line[1], line[2], line[3], line[4], line[5], line[6], line[7], line[8], line[9], line[10], line[11]));
                cnt++;
            }

            Console.WriteLine("Processing completed. {0} rows processed.", cnt);
            sr.Close();
            outputFile.Close();
            Console.ReadLine();
        }

        private static void ProcessGeonamesData()
        {
            var sr = new StreamReader(@"C:\Temp\GeoNamesData\Postal Codes\LT.txt");
            var outputFile = new StreamWriter(@"C:\Temp\GeoNamesData\Postal Codes\LtUnicode.txt", false, Encoding.Unicode);
            var cnt = 0;

            while (!sr.EndOfStream)
            {
                var strline = sr.ReadLine();

                if (strline == null)
                    continue;

               // var values = strline.Split('\t');
                //var line = new List<string> { "CA", values[0].Replace('"', ' ').Trim(), values[3].Replace('"', ' ').Trim(), "", "", "", "", "", "", values[1].Replace('"', ' ').Trim(), values[2].Replace('"', ' ').Trim(), "" };
                //outputFile.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}", line[0], line[1], line[2], line[3], line[4], line[5], line[6], line[7], line[8], line[9], line[10], line[11]));
                outputFile.WriteLine(strline);
                cnt++;
            }

            Console.WriteLine("Processing completed. {0} rows processed.", cnt);
            sr.Close();
            outputFile.Close();
            Console.ReadLine();
        }
    }
}
