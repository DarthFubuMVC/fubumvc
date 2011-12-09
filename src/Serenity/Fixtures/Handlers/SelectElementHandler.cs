using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using FubuCore;
using StoryTeller.Assertions;
using System.Linq;
using System.Collections.Generic;

namespace Serenity.Fixtures.Handlers
{
    public class SelectElementHandler : IElementHandler, IMatchingHandler
    {
        public bool Matches(IWebElement element)
        {
            return element.TagName.ToLower() == "select";
        }

        public void EnterData(ISearchContext context, IWebElement element, object data)
        {
            var options = findOptions(element);
            foreach (var option in options)
            {
                if (option.Text == data.ToString())
                {
                    option.Click();
                    return;
                }
            }

            foreach (var option in options)
            {
                if (option.GetAttribute("value") == data.ToString())
                {
                    option.Click();
                    return;
                }
            }

            var message = "Cannot find the desired option\nThe available options are\nDisplay/Key\n";
            foreach (var option in options)
            {
                message += "\n" + "{0}/{1}".ToFormat(option.Text, option.GetAttribute("value"));
            }

            StoryTellerAssert.Fail(message);

        }

        private static IEnumerable<IWebElement> findOptions(IWebElement element)
        {
            return element.FindElements(By.TagName("option"));
        }

        public string GetData(ISearchContext context, IWebElement element)
        {
            var selected = findSelected(element);

            if (selected == null) return "Nothing";

            var value = selected.GetAttribute("value");
            return value.IsEmpty() ? selected.Text : "{0}={1}".ToFormat(selected.Text, value);
        }

        private IWebElement findSelected(IWebElement element)
        {
            var options = findOptions(element);
            return options.FirstOrDefault(x => x.GetAttribute("selected").IsNotEmpty());
        }

        public bool MatchesData(IWebElement element, object expected)
        {
            var expectedValue = expected.ToString();
            var selected = findSelected(element);

            if (selected == null) return false;

            
            if (selected.GetAttribute("value") == expectedValue) return true;

            if (findOptions(element).Select(x => x.GetAttribute("value")).Contains(expectedValue))
            {
                return false;
            }

            return selected.Text == expectedValue;
        }
    }
}