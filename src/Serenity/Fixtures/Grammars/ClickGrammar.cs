using System.Collections.Generic;
using OpenQA.Selenium;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace Serenity.Fixtures.Grammars
{
    public class ClickGrammar : SimpleElementGesture
    {
        public ClickGrammar(GestureConfig config) : base(config)
        {
        }

        protected override void execute(IWebElement element, IDictionary<string, object> cellValues, IStep step, ITestContext context)
        {
            assertCondition(element.Enabled, DisabledElementMessage);
            assertCondition(element.Displayed, HiddenElementMessage);

            element.Click();
        }
    }
}