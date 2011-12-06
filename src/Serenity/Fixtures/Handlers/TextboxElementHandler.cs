using FubuCore;
using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public class TextboxElementHandler : IElementHandler
    {
        public bool Matches(IWebElement element)
        {
            // TODO --- Change to psuedo CSS class?  Less repeated logic
            return element.TagName.ToLower() == "input" && element.GetAttribute("type").ToLower() == "text";
        }

        public void EnterData(IWebElement element, object data)
        {
            while (element.GetAttribute("value").IsNotEmpty())
            {
                element.SendKeys(Keys.Backspace);
            }

            element.SendKeys(data as string ?? string.Empty);
        }

        public string GetData(IWebElement element)
        {
            return element.GetAttribute("value");
        }
    }
}