using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public abstract class ElementHandlerWrapper : IElementHandler
    {
        public static Func<IEnumerable<IElementHandler>> AllHandlers { get; set; }

        static ElementHandlerWrapper()
        {
            ResetAllHandlers();
        }

        public static void ResetAllHandlers()
        {
            AllHandlers = () => ElementHandlers.AllHandlers;
        }

        private IEnumerable<IElementHandler> FollowOnHandlers
        {
            get { return AllHandlers().SkipWhile(x => !ReferenceEquals(this, x)).Skip(1); }
        }

        public bool Matches(IWebElement element)
        {
            return WrapperMatches(element) && FollowOnHandlers.Any(x => x.Matches(element));
        }

        protected abstract bool WrapperMatches(IWebElement element);

        public abstract void EnterData(ISearchContext context, IWebElement element, object data);

        protected void EnterDataNested(ISearchContext context, IWebElement element, object data)
        {
            FollowOnHandlers.First(x => x.Matches(element)).EnterData(context, element, data);
        }

        public abstract string GetData(ISearchContext context, IWebElement element);

        protected string GetDataNested(ISearchContext context, IWebElement element)
        {
            return FollowOnHandlers.First(x => x.Matches(element)).GetData(context, element);
        }
    }
}