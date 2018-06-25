using System;

namespace LocationIndexer.Utilities
{
    public static class StringExtensions
    {
        public static bool ContainsDigits(this string value)
        {
            foreach (var c in value)
            {
                if (Char.IsDigit(c))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
