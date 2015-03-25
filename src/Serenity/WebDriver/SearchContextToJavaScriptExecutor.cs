using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace Serenity.WebDriver
{
    public class SearchContextToJavaScriptExecutor : ISearchContextToJavaScriptExecutor
    {
        public IJavaScriptExecutor Convert(ISearchContext context)
        {
            var executor = context as IJavaScriptExecutor;

            if (executor != null)
            {
                return executor;
            }

            var wrappedDriver = context as IWrapsDriver;

            if (wrappedDriver != null)
            {
                executor = wrappedDriver.WrappedDriver as IJavaScriptExecutor;
            }

            if (executor == null)
            {
                throw new ArgumentException("Search context must be an IJavaScriptExecutor or an IWrapsDriver that wraps an IJavaScriptExecutor", "context");
            }

            return executor;
        }
    }
}