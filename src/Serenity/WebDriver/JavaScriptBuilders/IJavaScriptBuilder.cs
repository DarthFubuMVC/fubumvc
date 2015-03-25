namespace Serenity.WebDriver.JavaScriptBuilders
{
    public interface IJavaScriptBuilder
    {
        bool Matches(object obj);
        string Build(object obj);
    }
}