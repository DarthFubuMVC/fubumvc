using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Serenity.Fixtures.Handlers;
using SeleniumBy = OpenQA.Selenium.By;

namespace Serenity.Fixtures
{
    public static class WebDriverExtensions
    {
        public static SeleniumBy ByCss(this string css)
        {
            return css.IsEmpty() ? null : By.CssSelector(css);
        }

        public static SeleniumBy ByName(this string name)
        {
            return name.IsEmpty() ? null : By.Name(name);
        }

        public static SeleniumBy ById(this string id)
        {
            return id.IsEmpty() ? null : By.Id(id);
        }

        // the interface just isn't very discoverable
        public static object InjectJavascript(this IWebDriver driver, string script)
        {
            return ((IJavaScriptExecutor)driver).ExecuteScript(script);
        }

        public static T InjectJavascript<T>(this IWebDriver driver, string script)
        {
            return (T)driver.InjectJavascript(script);
        }

        public static IWebElement FindElementOrNull(this ISearchContext context, By selector)
        {
            try
            {
                return context.FindElement(selector);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public static IWebElement FindElementByData(this IWebDriver driver, string attribute, string value)
        {
            return driver.FindElement(By.CssSelector(CssSelectorByData(attribute, value)));
        }

        public static ReadOnlyCollection<IWebElement> FindElementsByData(this IWebDriver driver, string attribute, string value)
        {
            return driver.FindElements(By.CssSelector(CssSelectorByData(attribute, value)));
        }

        public static string CssSelectorByData(string attribute, string value)
        {
            return "*[data-{0}={1}]".ToFormat(attribute, value);
        }

        public static IWebElement WaitUntil(this IWebDriver driver, Func<IWebElement> condition, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => condition());
        }

        public static readonly Type[] WaitForElementIgnoreExceptions =
        {
            typeof(NoSuchElementException),
            typeof(NotFoundException)
        };

        public static IWebElement WaitForElement(this ISearchContext context, By selector, TimeSpan? timeout = null, TimeSpan? pollingInterval = null)
        {
            try
            {
                return Wait.For(() => context.FindElement(selector),
                    pollingInterval: pollingInterval,
                    timeout: timeout,
                    ignoreExceptions: WaitForElementIgnoreExceptions);
            }
            catch (WebDriverTimeoutException timeoutException)
            {
                throw new NoSuchElementException("Unable to locate element: {0}".ToFormat(selector), timeoutException);
            }
        }

        public static IWebElement InputFor<T>(this ISearchContext context, Expression<Func<T, object>> property)
        {
            return context.FindElement(By.Name(property.ToAccessor().Name));
        }

        public static IWebElement LabelFor<T>(this ISearchContext context, Expression<Func<T, object>> property)
        {
            return context.FindElement(By.CssSelector("label[for='{0}']".ToFormat(property.ToAccessor().Name)));
        }

        public static string Data(this IWebElement element, string attribute)
        {
            return element.GetAttribute("data-{0}".ToFormat(attribute));
        }

        public static IWebElement Parent(this IWebElement element)
        {
            return element.FindElement(By.XPath(".."));
        }

        public static IEnumerable<string> GetClasses(this IWebElement element)
        {
            return element
                .GetAttribute("class")
                .Split(' ');
        }

        public static bool HasClass(this IWebElement element, string className)
        {
            return element
                .GetClasses()
                .Contains(className);
        }

        public static bool HasAttribute(this IWebElement element, string attributeName)
        {
            return element.GetAttribute(attributeName) != null;
        }

        public static string Value(this IWebElement element)
        {
            return element.GetAttribute("value");
        }

        public static string Id(this IWebElement element)
        {
            return element.GetAttribute("id");
        }

        public static string FindClasses(this IWebElement element, params string[] classes)
        {
            return classes.Where(c => element.HasClass(c)).Join(" ");
        }

        public static string GetData(this ISearchContext context, IWebElement element)
        {
            return ElementHandlers.FindHandler(element).GetData(context, element);
        }

        public static void SetData(this ISearchContext context, IWebElement element, string value)
        {
            ElementHandlers.FindHandler(element).EnterData(context, element, value);
        }
    }
}
