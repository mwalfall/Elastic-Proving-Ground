using LocationIndexer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationIndexer.Services
{
    public class CountryService : ICountryService
    {
        private List<string> _countryCodes = new List<string>();

        #region Constructor

        public CountryService(List<string> countryCodes)
        {
            _countryCodes = countryCodes;
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Returns valid upper case country codes.
        /// </summary>
        /// 
        public List<string> GetCountriesToIndex()
        {
            string countriesToIndex = System.Configuration.ConfigurationManager.AppSettings["CountriesToIndex"].ToUpper();

            if (string.IsNullOrWhiteSpace(countriesToIndex) || countriesToIndex.ToLower().Equals("all"))
                return _countryCodes;
            else
                return GetCountryCodesList(countriesToIndex);
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns valid country codes taken from the App.config file.
        /// </summary>
        /// <param name="countriesToIndex">List of country codes</param>
        /// 
        private List<string> GetCountryCodesList(string countriesToIndex)
        {
            var codesToIndexList = countriesToIndex.ToUpper().Split(',').ToList();
            foreach (var code in codesToIndexList)
            {
                if (!_countryCodes.Contains(code))
                    throw new ArgumentException(string.Format("An invalid country code was submiited. CountryCode: {0}", code));
            }
            return codesToIndexList;
        }
        #endregion Private Methods
    }
}
