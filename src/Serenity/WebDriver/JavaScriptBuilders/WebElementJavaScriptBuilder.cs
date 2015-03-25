using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace Serenity.WebDriver.JavaScriptBuilders
{
    public class WebElementJavaScriptBuilder : IJavaScriptBuilder
    {
        public const string Marker = "::arg::";
        public static readonly Regex MarkerRgx = new Regex(Regex.Escape(Marker));

        public bool Matches(object obj)
        {
            return obj is IWebElement;
        }

        public string Build(object obj)
        {
            return Marker;
        }
    }
}