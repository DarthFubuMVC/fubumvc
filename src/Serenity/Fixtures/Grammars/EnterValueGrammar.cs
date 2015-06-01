using System.Collections.Generic;
using OpenQA.Selenium;
using Serenity.Fixtures.Handlers;
using StoryTeller.Conversion;
using StoryTeller.Results;

namespace Serenity.Fixtures.Grammars
{
    public class EnterValueGrammar : SimpleElementGesture
    {
        public EnterValueGrammar(ScreenFixture fixture, GestureConfig def) : base(fixture, def)
        {
        }

        protected override IEnumerable<CellResult> execute(IWebElement element, StepValues values)
        {
            var handler = ElementHandlers.FindHandler(element);
            handler.EnterData(SearchContext, element, values.Get(Cell.Key));

            yield return CellResult.Ok(Cell.Key);
        }
    }
}