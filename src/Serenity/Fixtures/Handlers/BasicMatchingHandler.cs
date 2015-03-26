using OpenQA.Selenium;
using StoryTeller.Equivalence;

namespace Serenity.Fixtures.Handlers
{
    public class BasicMatchingHandler : IMatchingHandler
    {
        private readonly IElementHandler _inner;
        private static readonly EquivalenceChecker checker = new EquivalenceChecker(); 

        public BasicMatchingHandler(IElementHandler inner)
        {
            _inner = inner;
        }

        public bool MatchesData(IWebElement element, object expected)
        {
            var actual = _inner.GetData(null, element);
            return checker.IsEqual(expected, actual);
        }
    }
}
