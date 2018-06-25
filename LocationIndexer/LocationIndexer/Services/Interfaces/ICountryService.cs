using System;
using System.Collections.Generic;

namespace LocationIndexer.Services.Interfaces
{
    public interface ICountryService
    {
        /// <summary>
        /// Returns valid upper case country codes.
        /// </summary>
        /// 
        List<string> GetCountriesToIndex();
    }
}
