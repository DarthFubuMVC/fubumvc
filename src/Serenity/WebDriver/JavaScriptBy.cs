using System.Collections.Generic;
using System.Collections.ObjectModel;
using FubuCore;
using OpenQA.Selenium;

namespace Serenity.WebDriver
{
    public class JavaScriptBy : By
    {
        public dynamic JavaScript { get; private set; }

        public static ISearchContextToJavaScriptExecutor SearchContextConverter { get; set; }

        static JavaScriptBy()
        {
            ResetConverter();
        }

        public JavaScriptBy(dynamic javaScript)
        {
            JavaScript = javaScript;
        }

        public static void ResetConverter()
        {
            SearchContextConverter = new SearchContextToJavaScriptExecutor();
        }

        public override IWebElement FindElement(ISearchContext context)
        {
            var element = Execute<IWebElement>(context, JavaScript.Get(0));

            if (element != null)
            {
                return element;
            }

            throw new NoSuchElementException("No element found with JavaScript command: " + JavaScript.Statement);
        }

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            var rawCollection = Execute<object>(context, JavaScript.Get());

            var collection = rawCollection as ReadOnlyCollection<IWebElement>;

            //Unlike FindElement, FindElements does not throw an exception if no elements are found
            //and instead returns an empty list
            return collection ?? (new ReadOnlyCollection<IWebElement>(new List<IWebElement>()));
        }

        static T Execute<T>(ISearchContext context, dynamic jquery) where T : class
        {
            var executor = SearchContextConverter.Convert(context);
            return jquery.ExecuteAndGet<T>(executor);
        }

        public override string ToString()
        {
            return "By.JavaScript: {0}".ToFormat((string) JavaScript.Statement);
        }
    }
}