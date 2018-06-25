using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationDulpicateNameAnalyzer.Services
{
    public class FormatSettingService
    {
        private AdoClientService _adoClientService;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adoClientService">AdoClientService object</param>
        public FormatSettingService(AdoClientService adoClientService)
        {
            _adoClientService = adoClientService;
        }
        #endregion Constructor

        #region Public Method

        /// <summary>
        /// executes the process to determine the formatting code for duplicate FormattedNames.
        /// </summary>
        public void Configure()
        {
            var countryCodes = _adoClientService.GetCountryCodes();

            foreach (var countryCode in countryCodes)
            {
                Console.WriteLine(string.Format("--> Processing: '{0}'", countryCode));

                var duplicateFormattedNames = _adoClientService.GetDuplicateFormattedNames(countryCode);
                if (duplicateFormattedNames.Count() == 0)
                    continue;

                var formattedNamesObjects = _adoClientService.GetDuplicates(duplicateFormattedNames, countryCode);

                foreach (var formattedName in duplicateFormattedNames)
                {
                    var names = formattedNamesObjects.Where(x => x.FormattedNameNon.Equals(formattedName)).ToList();
                    var typeIds = names.Select(x => x.TypeId).Distinct().ToList();

                    if (typeIds.Count() == 1)
                        _adoClientService.SetFormatting(names.ToList(),1);
                    if (typeIds.Count() == 2)
                    {
                        if (names.Count() == 2)
                            _adoClientService.SetFormatting(names.ToList(), 2);
                        else
                            _adoClientService.SetFormatting(names.ToList(), 3);
                    }
                }
            }
        }
        #endregion Public Method
    }
}
