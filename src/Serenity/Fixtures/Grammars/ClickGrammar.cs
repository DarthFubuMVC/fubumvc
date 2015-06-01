using System.Collections.Generic;
using OpenQA.Selenium;
using StoryTeller.Conversion;
using StoryTeller.Results;

namespace Serenity.Fixtures.Grammars
{
    public class ClickGrammar : SimpleElementGesture
    {
        public ClickGrammar(ScreenFixture fixture, GestureConfig config) : base(fixture, config)
        {
        }

        protected override IEnumerable<CellResult> execute(IWebElement element, StepValues values)
        {
            assertCondition(element.Enabled, DisabledElementMessage);
            assertCondition(element.Displayed, HiddenElementMessage);

            element.Click();

            yield return CellResult.Ok(Cell.Key);
        }

    }
}