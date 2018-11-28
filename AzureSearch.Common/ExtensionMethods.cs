using System;
using System.Collections.Generic;

namespace AzureSearch.Common
{
    /// <summary>
    /// TODO Move this class to common modules
    /// </summary>
    public static class ExtensionMethods
    {
        #region string
        /// <summary>
        /// Returns the text in a string between two delimiters..
        /// If either delimiter is not found or the first delimiter is found after the past delimiter in the text, returns null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="firstDelimiter"></param>
        /// <param name="lastDelimiter"></param>
        /// <returns></returns>
        public static string EmGetTextBetweenTwoDelimiters(this string value, char firstDelimiter, char lastDelimiter)
        {
            int indexFirst = value.IndexOf(firstDelimiter);
            if (indexFirst < 0)
            {
                return null;
            }
            int indexLast = value.LastIndexOf(lastDelimiter);
            if (indexLast < 0)
            {
                return null;
            }
            if (indexFirst > indexLast)
            {
                return null;
            }
            if (indexLast == indexFirst)
            {
                return string.Empty;
            }
            return value.Substring(indexFirst + 1, indexLast - indexFirst - 1);
        }

        /// <summary>
        /// If the delimiter is not found, returns null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string EmGetTextBeforeDelimiter(this string value, char delimiter)
        {
            if (value == null)
            {
                return null;
            }
            int index = value.IndexOf(delimiter);
            if (index < 0)
            {
                return null;
            }
            if (index == 0)
            {
                return string.Empty;
            }
            return value.Substring(0, index);
        }

        /// <summary>
        /// Compares two string values ignoring case.
        /// </summary>
        /// <param name="firstString">
        /// The first string.
        /// </param>
        /// <param name="secondString">
        /// The second string.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool EmCompareIgnoreCase(this string firstString, string secondString)
        {
            if (string.Compare(firstString, secondString, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies that a string starts a specific suffix with ignore case.
        /// </summary>
        /// <param name="mainString">
        /// The main string.
        /// </param>
        /// <param name="startsWidthString">
        /// The starts width string.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool EmStartsWithIgnoreCase(this string mainString, string startsWidthString)
        {
            return mainString.StartsWith(startsWidthString, StringComparison.OrdinalIgnoreCase);
        }
        public static bool EmContainsIgnoreCase(this string value, string containsString)
        {
            if (string.IsNullOrEmpty(containsString))
            {
                throw new ArgumentException("containsString has to be a non-null value with a length greater than 0");
            }
            return (value.IndexOf(containsString, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        #endregion
    }
    /// <summary>
    /// Not an extension method per se, but kind of go with the case insensitive extension methods.
    /// Use this for instance with LINQ's IEnumerable.Contains() method for instance.
    /// </summary>
    public class EmStringEqualityComparerCaseInsensitive : IEqualityComparer<string>
    {
        public bool Equals(string left, string right)
        {
            return left.EmCompareIgnoreCase(right);
        }

        public int GetHashCode(string value)
        {
            int hash = 0;
            for (int c = 0; c < value.Length; c++)
            {
                hash += value[c];
            }
            return hash;
        }
    }

}
