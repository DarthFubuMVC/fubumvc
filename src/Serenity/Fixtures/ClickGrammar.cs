using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace Serenity.Fixtures
{
    public class ClickGrammar : SimpleElementGesture
    {
        public ClickGrammar(GestureConfig config) : base(config)
        {
        }

        protected override void execute(IWebElement element, IDictionary<string, object> cellValues)
        {
            assertCondition(element.Enabled, DisabledElementMessage);
            assertCondition(element.Displayed, HiddenElementMessage);

            element.Click();
        }
    }
}