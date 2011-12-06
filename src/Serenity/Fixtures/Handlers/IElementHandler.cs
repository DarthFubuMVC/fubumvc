using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public interface IElementHandler
    {
        bool Matches(IWebElement element);
        void EnterData(IWebElement element, object data);
        string GetData(IWebElement element);
    }
}