using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;
using Serenity.Fixtures.Grammars;
using StoryTeller;
using StoryTeller.Grammars;

namespace Serenity.Fixtures
{
    public class ScreenFixture : SerenityFixture
    {
        private readonly Stack<ISearchContext> _searchContexts = new Stack<ISearchContext>();

        public ISearchContext SearchContext
        {
            get
            {
                if (_searchContexts.Count == 0)
                {
                    return Driver;
                }

                return _searchContexts.Peek();
            }
        }

        protected void PushElementContext(ISearchContext context)
        {
            _searchContexts.Push(context);
        }

        protected void PushElementContext(By selector)
        {
            var element = SearchContext.FindElement(selector);
            StoryTellerAssert.Fail(element == null, () => "Unable to find element with " + selector);

            PushElementContext(element);
        }

        protected void PopElementContext()
        {
            if (_searchContexts.Any()) _searchContexts.Pop();
        }

        protected NavigationDriver Navigation
        {
            get { return Retrieve<NavigationDriver>(); }
        }

        protected internal IWebDriver Driver
        {
            get { return Browser.Driver; }
        }

        protected IBrowserLifecycle Browser
        {
            get { return Retrieve<IBrowserLifecycle>(); }
        }

        protected string RootUrl
        {
            get { return Context.Service<FubuRuntime>().BaseAddress; }
        }

        protected IUrlRegistry Urls
        {
            get { return Context.Service<IUrlRegistry>(); }
        }

        public override sealed void SetUp()
        {
            // TODO -- optimize this a bit by finding IBrowserLifecycle early

            _searchContexts.Clear();

            beforeRunning();
        }

        protected virtual void beforeRunning()
        {
        }

        protected IGrammar Click(By selector = null, string id = null, string css = null, string name = null,
            string label = null, string template = null)
        {
            var by = selector ?? id.ById() ?? css.ByCss() ?? name.ByName();

            if (by == null)
                throw new InvalidOperationException("Must specify either the selector, css, or name property");

            label = label ?? by.ToString().Replace("By.", "");


            return Do(label, c => SearchContext.FindElement(by).Click());
        }

        // TODO -- UT this some how
        // TODO -- Convert to use Serenity.WebDriver.JavaScript
        protected IGrammar JQueryClick(string template, string id = null, string className = null, string css = null,
            string tagName = null)
        {
            var command = buildJQuerySearch(css, id, className, tagName);

            return Do(template, c => { Retry.Twice(() => Driver.InjectJavascript(command)); });
        }

        private static string buildJQuerySearch(string css, string id, string className, string tagName)
        {
            var search = css;

            if (id.IsNotEmpty())
            {
                search = "#" + id;
            }

            if (className.IsNotEmpty())
            {
                search = "." + className;
            }

            if (tagName.IsNotEmpty())
            {
                search = tagName + search;
            }

            return "$('{0}').click();".ToFormat(search);
        }

        protected void ClickWithJQuery(string id = null, string className = null, string css = null,
            string tagName = null)
        {
            var command = buildJQuerySearch(css, id, className, tagName);

            Retry.Twice(() => Driver.InjectJavascript(command));
        }


        protected void waitForElement(By elementSearch, int millisecondPolling = 500, int timeoutInMilliseconds = 5000)
        {
            SearchContext.WaitForElement(elementSearch, TimeSpan.FromMilliseconds(timeoutInMilliseconds),
                TimeSpan.FromMilliseconds(millisecondPolling));
        }

        protected string GetData(IWebElement element)
        {
            return SearchContext.GetData(element);
        }

        protected string GetData(By finder)
        {
            var element = SearchContext.FindElement(finder);
            return SearchContext.GetData(element);
        }

        protected void SetData(IWebElement element, string data)
        {
            SearchContext.SetData(element, data);
        }

        protected void SetData(By finder, string data)
        {
            var element = SearchContext.FindElement(finder);
            SetData(element, data);
        }

        protected IGrammar BrowserIsAt(object model, string title, string categoryOrHttpMethod = null)
        {
            return BrowserIsAt(x => x.UrlFor(model, categoryOrHttpMethod), title);
        }

        protected IGrammar BrowserIsAt<T>(string title, string categoryOrHttpMethod = null) where T : class
        {
            return BrowserIsAt(x => x.UrlFor<T>(categoryOrHttpMethod), title);
        }

        protected IGrammar BrowserIsAt(Type handlerType, string title, MethodInfo method = null,
            string categoryOrHttpMethodOrHttpMethod = null)
        {
            return BrowserIsAt(x => x.UrlFor(handlerType, method, categoryOrHttpMethodOrHttpMethod), title);
        }

        protected IGrammar BrowserIsAt<TController>(Expression<Action<TController>> expression, string title,
            string categoryOrHttpMethod = null)
        {
            return BrowserIsAt(x => x.UrlFor(expression, categoryOrHttpMethod), title);
        }

        protected IGrammar BrowserIsAt(Type modelType, RouteParameters parameters, string title,
            string categoryOrHttpMethod = null)
        {
            return BrowserIsAt(x => x.UrlFor(modelType, parameters, categoryOrHttpMethod), title);
        }

        protected IGrammar BrowserIsAt(Func<IUrlRegistry, string> toUrl, string title)
        {
            return new FactGrammar(title, c => Driver.Url.Matches(toUrl(Urls)));
        }

        protected string[] JavascriptErrorsInBrowser()
        {
            return Driver
                .InjectJavascript("return SerenityErrorCollector.errors")
                .As<IEnumerable<object>>().Select(x =>
                {
                    if (x is Dictionary<string, object>)
                    {
                        var dict = x.As<Dictionary<string, object>>();
                        return dict.Select(pair => "{0} = '{1}'".ToFormat(pair.Key, pair.Value)).Join(", ");
                    }

                    return x.ToString();
                }).ToArray();
        } 
    }

    public class ScreenFixture<T> : ScreenFixture
    {
        protected void enterValue(Expression<Func<T, object>> expression, string value)
        {
            // TODO -- use the field naming convention?
            var name = expression.ToAccessor().Name;
            SetData(By.Name(name), value);
        }

        protected string readValue(Expression<Func<T, object>> expression)
        {
            var name = expression.ToAccessor().Name;
            return GetData(By.Name(name));
        }

        private GestureConfig getGesture(Expression<Func<T, object>> expression, string label = null, string key = null)
        {
            // TODO -- later on, use the naming convention from fubu instead of pretending
            // that this rule is always true
            var config = GestureForProperty(expression);
            if (key.IsNotEmpty())
            {
                config.CellName = key;
            }

            return config;
        }


        protected IGrammar EnterScreenValue(Expression<Func<T, object>> expression, string label = null,
            string key = null)
        {
            var config = getGesture(expression, label, key);

            config.Template = "Enter {" + config.CellName + "} for " + config.CellName;
            config.Description = "Enter data for property " + expression.ToAccessor().Name;

            return new EnterValueGrammar(this, config);
        }

        protected IGrammar CheckScreenValue(Expression<Func<T, object>> expression, string label = null,
            string key = null)
        {
            var config = getGesture(expression, label, key);

            config.Template = "The text of " + (label ?? config.CellName) + " should be {" + config.CellName + "}";
            config.Description = "Check data for property " + expression.ToAccessor().Name;

            return new CheckValueGrammar(this, config);
        }


        protected GestureConfig GestureForProperty(Expression<Func<T, object>> expression)
        {
            return GestureConfig.ByProperty(expression);
        }

        protected void EditableElement(Expression<Func<T, object>> expression, string label = null)
        {
            var accessor = expression.ToAccessor();
            var name = accessor.Name;

            this["Check" + name] = CheckScreenValue(expression, label);
            this["Enter" + name] = EnterScreenValue(expression, label);
        }

        protected void EditableElementsForAllImmediateProperties()
        {
            typeof (T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead && x.CanWrite)
                .Each(prop =>
                {
                    var accessor = new SingleProperty(prop);
                    var expression = accessor.ToExpression<T>();

                    EditableElement(expression);
                });
        }

        protected bool AssertIsOnScreen<TInput>(TInput input)
        {
            var expected = Urls.UrlFor(input, "GET");
            var actual = Driver.Url;

            if (!expected.Matches(Driver.Url))
            {
                StoryTellerAssert.Fail("The actual Url of the browser is " + actual.Canonize());
            }

            return true;
        }

        protected bool AssertIsOnScreen(string expected)
        {
            var actual = Driver.Url;

            if (!expected.Matches(Driver.Url))
            {
                StoryTellerAssert.Fail("The actual Url of the browser is " + actual.Canonize());
            }

            return true;
        }

        protected bool AssertIsNotOnScreen<TInput>(TInput input)
        {
            var actual = new Uri(Driver.Url).AbsolutePath;
            var expected = Urls.UrlFor(input, "GET");

            return !expected.Matches(actual);
        }
    }
}