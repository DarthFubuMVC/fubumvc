using System;
using OpenQA.Selenium;

namespace Serenity.Fixtures.Handlers
{
    public class DefaultElementHandler : IElementHandler
    {
        public bool Matches(IWebElement element)
        {
            return true;
        }

        public void EnterData(ISearchContext context, IWebElement element, object data)
        {

            throw new NotImplementedException();
        }

        public string GetData(ISearchContext context, IWebElement element)
        {
            return element.Text;
        }
    }
}