using System.Collections.Generic;
using OpenQA.Selenium;
using Serenity.Fixtures.Handlers;
using StoryTeller.Conversion;
using StoryTeller.Results;

namespace Serenity.Fixtures.Grammars
{
    public class CheckValueGrammar : SimpleElementGesture
    {
        public CheckValueGrammar(ScreenFixture fixture, GestureConfig config) : base(fixture, config)
        {
            
        }

        protected override IEnumerable<CellResult> execute(IWebElement element, StepValues values)
        {
            var handler = ElementHandlers.FindHandler(element);
            var expectedValue = values.Get(Cell.Key);

            var matchingHandler = handler as IMatchingHandler ?? new BasicMatchingHandler(handler);
            if (matchingHandler.MatchesData(element, expectedValue))
            {
                yield return new CellResult(Cell.Key, ResultStatus.success);
            }
            else
            {
                yield return new CellResult(Cell.Key, ResultStatus.failed){actual = handler.GetData(SearchContext, element)};
            }
        }
    }
}