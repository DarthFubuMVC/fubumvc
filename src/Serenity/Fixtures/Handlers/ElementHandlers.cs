using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public static class ElementHandlers
    {
        private static readonly IList<IElementHandler> _handlers = new List<IElementHandler>();
        private static readonly IList<IElementHandler> _defaultHandlers = new List<IElementHandler>{new CheckboxHandler(),new SelectElementHandler(), new TextboxElementHandler(), new TextAreaElementHandler(), new DefaultElementHandler()};

        public static IList<IElementHandler> Handlers
        {
            get { return _handlers; }
        }

        public static IEnumerable<IElementHandler> AllHandlers
        {
            get { return _handlers.Union(_defaultHandlers); }
        } 

        public static IElementHandler FindHandler(IWebElement element)
        {
            return AllHandlers.First(x => x.Matches(element));
        }
    }
}