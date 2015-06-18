using OpenQA.Selenium;

namespace Serenity
{
    /// <summary>
    /// Called immediately after navigating the browser to a new url.
    /// Useful as a way to deal with login pages in a web application
    /// </summary>
    public interface IAfterNavigation
    {
        void AfterNavigation(IWebDriver driver, string desiredUrl);
    }
}