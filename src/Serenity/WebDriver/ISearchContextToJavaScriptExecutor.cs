using OpenQA.Selenium;

namespace Serenity.WebDriver
{
    public interface ISearchContextToJavaScriptExecutor
    {
        IJavaScriptExecutor Convert(ISearchContext context);
    }
}