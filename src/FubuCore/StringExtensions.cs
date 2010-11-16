using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;

namespace FubuCore
{
    public static class StringExtensions
    {
        public static string ToFullPath(this string path)
        {
            return Path.GetFullPath(path);
        }

        public static string PathRelativeTo(this string path, string root)
        {
            var pathParts = path.ToLower().Replace('/', '\\').Split('\\').ToList();
            var rootParts = root.ToLower().Replace('/', '\\').Split('\\').ToList();

            var length = pathParts.Count > rootParts.Count ? rootParts.Count : pathParts.Count;
            for (int i = 0; i < length; i++)
            {
                if (pathParts.First() == rootParts.First())
                {
                    pathParts.RemoveAt(0);
                    rootParts.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < rootParts.Count; i++)
            {
                pathParts.Insert(0, "..");
            }

            return FileSystem.Combine(pathParts.ToArray());
        }

        public static bool IsEmpty(this string stringValue)
        {
            return string.IsNullOrEmpty(stringValue);
        }

        public static bool IsNotEmpty(this string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue);
        }

        public static bool ToBool(this string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue)) return false;

            return bool.Parse(stringValue);
        }

        public static string ToFormat(this string stringFormat, params object[] args)
        {
            return String.Format(stringFormat, args);
        }

        /// <summary>
        /// Performs a case-insensitive comparison of strings
        /// </summary>
        public static bool EqualsIgnoreCase(this string thisString, string otherString)
        {
            return thisString.Equals(otherString, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Converts the string to Title Case
        /// </summary>
        public static string Capitalize(this string stringValue)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stringValue);
        }

        public static string HtmlAttributeEncode(this string unEncoded)
        {
            return HttpUtility.HtmlAttributeEncode(unEncoded);
        }

        public static string HtmlEncode(this string unEncoded)
        {
            return HttpUtility.HtmlEncode(unEncoded);
        }

        public static string HtmlDecode(this string encoded)
        {
            return HttpUtility.HtmlDecode(encoded);
        }

        public static string UrlEncode(this string unEncoded)
        {
            return HttpUtility.UrlEncode(unEncoded);
        }

        public static string UrlDecode(this string encoded)
        {
            return HttpUtility.UrlDecode(encoded);
        }

        /// <summary>
        /// Formats a multi-line string for display on the web
        /// </summary>
        /// <param name="plainText"></param>
        public static string ConvertCRLFToBreaks(this string plainText)
        {
            return new Regex("(\r\n|\n)").Replace(plainText, "<br/>");
        }

        /// <summary>
        /// Returns a DateTime value parsed from the <paramref name="dateTimeValue"/> parameter.
        /// </summary>
        /// <param name="dateTimeValue">A valid, parseable DateTime value</param>
        /// <returns>The parsed DateTime value</returns>
        public static DateTime ToDateTime(this string dateTimeValue)
        {
            return DateTime.Parse(dateTimeValue);
        }

        public static string ToGmtFormattedDate(this DateTime date)
        {
            return date.ToString("yyyy'-'MM'-'dd hh':'mm':'ss tt 'GMT'");
        }

        public static string[] ToDelimitedArray(this string content)
        {
            return content.ToDelimitedArray(',');
        }

        public static string[] ToDelimitedArray(this string content, char delimiter)
        {
            string[] array = content.Split(delimiter);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }

            return array;
        }
    }
}