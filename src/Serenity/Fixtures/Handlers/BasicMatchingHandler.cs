using OpenQA.Selenium;
using StoryTeller.Engine;

namespace Serenity.Fixtures.Handlers
{
    public class BasicMatchingHandler : IMatchingHandler
    {
        private readonly IElementHandler _inner;
        private readonly ITestContext _context;

        public BasicMatchingHandler(IElementHandler inner, ITestContext context)
        {
            _inner = inner;
            _context = context;
        }

        public bool MatchesData(IWebElement element, object expected)
        {
            return _context.Matches(expected, _inner.GetData(null, element));
        }
    }
}
