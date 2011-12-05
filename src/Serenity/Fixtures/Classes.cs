
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using FubuCore;
using System.Linq;

namespace Serenity.Fixtures
{


    public interface IElementHandler
    {
        bool Matches(IWebElement element);
        void EnterData(IWebElement element, object data);
        string GetData(IWebElement element);
    }

    public class DefaultElementHandler : IElementHandler
    {
        public bool Matches(IWebElement element)
        {
            return true;
        }

        public void EnterData(IWebElement element, object data)
        {
            throw new NotImplementedException();
        }

        public string GetData(IWebElement element)
        {
            return element.Text;
        }
    }

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
            throw new NotImplementedException();
        }
    }

    public static class ElementHandlers
    {
        private static readonly IList<IElementHandler> _handlers = new List<IElementHandler>();
        private static readonly IList<IElementHandler> _defaultHandlers = new List<IElementHandler>{new TextboxElementHandler(), new DefaultElementHandler()};

        public static IList<IElementHandler> Handlers
        {
            get { return _handlers; }
        }

        public static IElementHandler FindHandler(IWebElement element)
        {
            return _handlers.Union(_defaultHandlers).First(x => x.Matches(element));
        }
    }
}
