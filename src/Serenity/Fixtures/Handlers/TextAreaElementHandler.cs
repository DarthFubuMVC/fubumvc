using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;
using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public class TextAreaElementHandler : IElementHandler
    {
        public bool Matches(IWebElement element)
        {
            return element.TagName.ToLower() == "textarea";
        }

        public void EnterData(ISearchContext context, IWebElement element, object data)
        {
            if (element.Text.IsNotEmpty() || element.GetAttribute("value").IsNotEmpty())
            {
                element.Clear();
            }
                

            element.SendKeys(data as string ?? string.Empty);
        }

        public string GetData(ISearchContext context, IWebElement element)
        {
            return element.GetAttribute("value") ?? element.Text;
        }
    }
}
