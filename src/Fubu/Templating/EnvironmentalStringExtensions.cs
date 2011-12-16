using System;

namespace Fubu.Templating
{
    public static class EnvironmentalStringExtensions
    {
        private static readonly string[] Splitters = { "\r\n", "\n" };
        public static string[] SplitOnNewLine(this string value)
        {
            return value.Split(Splitters, StringSplitOptions.None);
        }
    }
}