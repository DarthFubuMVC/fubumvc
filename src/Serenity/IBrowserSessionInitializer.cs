using OpenQA.Selenium;

namespace Serenity
{
    public interface IBrowserSessionInitializer
    {
        void InitializeSession(IWebDriver driver);
    }
}