using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public interface IMatchingHandler
    {
        bool MatchesData(IWebElement element, object expected);
    }
}