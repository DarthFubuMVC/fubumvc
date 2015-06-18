using OpenQA.Selenium;

namespace Serenity
{
    public class NulloAfterNavigation : IAfterNavigation
    {
        public void AfterNavigation(IWebDriver driver, string desiredUrl)
        {
            // Do nothing!
        }
    }
}