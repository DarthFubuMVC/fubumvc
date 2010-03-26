using System;

namespace FubuCore
{
    public static class StringExtensions
    {

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
        
    }
}