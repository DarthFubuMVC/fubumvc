using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public interface IElementHandler
    {
        bool Matches(IWebElement element);
        void EnterData(ISearchContext context, IWebElement element, object data);
        string GetData(ISearchContext context, IWebElement element);
    }
}