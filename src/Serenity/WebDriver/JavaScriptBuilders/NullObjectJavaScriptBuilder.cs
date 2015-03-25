namespace Serenity.WebDriver.JavaScriptBuilders
{
    public class NullObjectJavaScriptBuilder : IJavaScriptBuilder
    {
        public bool Matches(object obj)
        {
            return obj == null;
        }

        public string Build(object obj)
        {
            return "null";
        }
    }
}