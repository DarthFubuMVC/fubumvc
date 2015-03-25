using FubuCore;

namespace Serenity.WebDriver.JavaScriptBuilders
{
    public class StringJavaScriptBuilder : IJavaScriptBuilder
    {
        public bool Matches(object obj)
        {
            return obj is string;
        }

        public string Build(object obj)
        {
            return "\"{0}\"".ToFormat(obj);
        }
    }
}