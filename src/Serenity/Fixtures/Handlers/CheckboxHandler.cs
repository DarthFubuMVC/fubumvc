using System;
using OpenQA.Selenium;
using FubuCore;

namespace Serenity.Fixtures.Handlers
{
    public class CheckboxHandler : IElementHandler
    {
        public bool Matches(IWebElement element)
        {
            return element.TagName.Equals("input", StringComparison.InvariantCultureIgnoreCase) &&
                   element.GetAttribute("type").Equals("checkbox", StringComparison.InvariantCultureIgnoreCase);
        }

        public void EnterData(ISearchContext context, IWebElement element, object data)
        {
            bool value;
            bool.TryParse(data.ToString(), out value);

            var isChecked = IsChecked(element);

            if (value != isChecked)
            {
                element.Click();
            }
        }

        public string GetData(ISearchContext context, IWebElement element)
        {
            return IsChecked(element).ToString();
        }

        public static bool IsEnabled(IWebElement checkbox)
        {
            return checkbox.GetAttribute("disabled") == null;
        }

        public static bool IsChecked(IWebElement checkbox)
        {
            // This is a fun one. The value of the checked attribute changes depending on the browser?
            var @checked = checkbox.GetAttribute("checked");
            return @checked == "checked" || @checked == "true";
        }

        public static void Check(IWebElement checkbox)
        {
            if (!IsChecked(checkbox))
            {
                checkbox.Click();
            }
        }
    }
}