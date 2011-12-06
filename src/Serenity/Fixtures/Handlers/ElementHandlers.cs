using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public static class ElementHandlers
    {
        private static readonly IList<IElementHandler> _handlers = new List<IElementHandler>();
        private static readonly IList<IElementHandler> _defaultHandlers = new List<IElementHandler>{new SelectElementHandler(), new TextboxElementHandler(), new DefaultElementHandler()};

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