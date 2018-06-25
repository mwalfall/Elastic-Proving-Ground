using LocationDulpicateNameAnalyzer.Services;
using System;

namespace LocationDulpicateNameAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var adoClientService = new AdoClientService();
            var esClient = new ElasticClientService();

            //PopulateUniqueNames();
            //PerformFormattedNameAnalysis(adoClientService);
            //SetFormattingCodesForDuplicateNames(adoClientService);
            ResolveDuplicateMappings(adoClientService);
            //MissingLocationAnalyzer(adoClientService, esClient);
            Console.ReadLine();
        }

        private static void MissingLocationAnalyzer(AdoClientService adoClientService, ElasticClientService esClient)
        {
            var analyzer = new MissingLocationsIdentifier(adoClientService, esClient);
            analyzer.Process();
        }

        private static void ResolveDuplicateMappings(AdoClientService adoClientService)
        {
            var resolver = new DuplicateLocationMappingResolver(adoClientService);
            resolver.Resolve();

        }
        private static void PerformFormattedNameAnalysis(AdoClientService adoClientService)
        {
            var namesService = new NamesService(adoClientService);
            namesService.Analyze();
        }

        private static void SetFormattingCodesForDuplicateNames(AdoClientService adoClientService)
        {
            var formattingService = new FormatSettingService(adoClientService);
            formattingService.Configure();
        }

        private static void PopulateUniqueNames()
        {
            var adoClientService = new AdoClientService();
            var namingService = new NamingService(adoClientService);

            var countryCodes = adoClientService.GetCountryCodes();
            foreach(var countryCode in countryCodes)
            {
                namingService.SetUniqueNames("en", countryCode);
            }           
        }

        //private static void BuildFormattedNames()
        //{
        //    var options = new ConfigurationOptions
        //    {
        //        CountryCodes = new List<string> {   "AD","AE","AF","AG","AI","AL","AM","AO","AQ","AR","AS","AT","AU","AW","AX","AZ",
        //                                            "BA","BB","BD","BE","BF","BG","BH","BI","BJ","BL","BM","BN","BO","BQ","BR","BS","BT","BW","BY","BZ",
        //                                            "CA","CC","CD","CF","CG","CH","CI","CK","CL","CN","CM","CO","CR","CU","CV","CW","CX","CY","CZ",
        //                                            "DE","DJ","DK","DM","DO","DZ",
        //                                            "EC","EE","EG","EH","ER","ES","ET",
        //                                            "FI","FJ","FK","FM","FO","FR",
        //                                            "GA","GB","GD","GE","GF","GG","GH","GI","GL","GM","GN","GP","GQ","GR","GS","GT","GU","GW","GY",
        //                                            "HK","HN","HR","HT","HU",
        //                                            "ID","IE","IL","IM","IN","IO","IQ","IR","IS","IT",
        //                                            "JE","JM","JO","JP",
        //                                            "KE","KG","KH","KI","KM","KN","KP","KR","KW","KY","KZ",
        //                                            "LA","LB","LC","LI","LK","LR","LS","LT","LU","LV","LY",
        //                                            "MA","MC","MD","ME","MF","MG","MH","MK","ML","MM","MN","MO","MP","MQ","MR","MS","MT","MU","MV","MW","MX","MY","MZ",
        //                                            "NA","NC","NE","NF","NG","NI","NL","NO","NP","NR","NU","NZ",
        //                                            "OM",
        //                                            "PA","PE","PF","PG","PH","PK","PL","PM","PN","PR","PS","PT","PW","PY",
        //                                            "QA",
        //                                            "RE","RO","RS","RU","RW",
        //                                            "SA","SB","SC","SD","SE","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SR","SS","ST","SV","SX","SY","SZ",
        //                                            "TC","TD","TF","TG","TH","TJ","TK","TL","TM","TN","TO","TR","TT","TV","TW","TZ",
        //                                            "UA","UG","UM","US","UY","UZ",
        //                                            "VA","VC","VE","VG","VI","VN","VU",
        //                                            "WF","WS",
        //                                            "XK",
        //                                            "YE","YT",
        //                                            "ZA","ZM","ZW"},
        //        XmlDocumentPath = @"\\TMPPFDB01D\PlatformData\GeoNames Data\LocationXml\en"
        //    };

        //    foreach (var countryCode in options.CountryCodes)
        //    {
        //        Console.WriteLine(string.Format("Processing: {0}.", countryCode));
        //        var xmlDataService = new XmlDataService();
        //        Console.WriteLine("Obtaining documents...");
        //        var documents = xmlDataService.Get<LocationFormattedName>(options, countryCode);

        //        if (documents.Any())
        //        {
        //            Console.WriteLine("Writing documents to db....");
        //            var adoClientService = new AdoClientService();
        //            adoClientService.SetLocationFormattedNames(documents.ToList(), DataTableBuilder.Get());
        //        }
        //        Console.WriteLine("--");
        //    }

        //    Console.WriteLine("**** Procesing completed successfully. ****");
        //    Environment.Exit(0);
        //}
    }
}
