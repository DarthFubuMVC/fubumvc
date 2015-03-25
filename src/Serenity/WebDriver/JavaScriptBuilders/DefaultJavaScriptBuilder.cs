namespace Serenity.WebDriver.JavaScriptBuilders
{
    public class DefaultJavaScriptBuilder : IJavaScriptBuilder
    {
        public bool Matches(object obj)
        {
            return true;
        }

        public string Build(object obj)
        {
            if (obj == null)
            {
                return "null";
            }

            return obj.ToString();
        }
    }
}