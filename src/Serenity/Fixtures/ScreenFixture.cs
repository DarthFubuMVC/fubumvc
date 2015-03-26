using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;
using StoryTeller;
using StoryTeller.Engine;

namespace Serenity.Fixtures
{
    public class ScreenFixture : Fixture
    {
        private IApplicationUnderTest _application;


        protected IApplicationUnderTest Application
        {
            get { return _application; }
        }

        protected NavigationDriver Navigation
        {
            get { return _application.Navigation; }
        }

        protected EndpointDriver Endpoints
        {
            get { return _application.Endpoints(); }
        }

        protected IWebDriver Driver
        {
            get { return _application.Driver; }
        }

        public override sealed void SetUp(ITestContext context)
        {
            _application = context.Retrieve<IApplicationUnderTest>();

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


            return Do(label, () => Driver.FindElement(by).Click());
        }

        // TODO -- UT this some how
        // TODO -- Convert to use Serenity.WebDriver.JavaScript
        protected IGrammar JQueryClick(string template, string id = null, string className = null, string css = null,
            string tagName = null)
        {
            string command = buildJQuerySearch(css, id, className, tagName);

            return Do(template, () => { Retry.Twice(() => Driver.InjectJavascript(command)); });
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
            string command = buildJQuerySearch(css, id, className, tagName);

            Retry.Twice(() => Driver.InjectJavascript(command));
        }


        protected void waitForElement(By elementSearch, int millisecondPolling = 500, int timeoutInMilliseconds = 5000)
        {
            Driver.WaitForElement(elementSearch, TimeSpan.FromMilliseconds(timeoutInMilliseconds),
                TimeSpan.FromMilliseconds(millisecondPolling));
        }

        protected string GetData(IWebElement element)
        {
            return Driver.GetData(element);
        }

        protected string GetData(By finder)
        {
            var element = Driver.FindElement(finder);
            return Driver.GetData(element);
        }

        protected void SetData(IWebElement element, string data)
        {
            Driver.SetData(element, data);
        }

        protected void SetData(By finder, string data)
        {
            var element = Driver.FindElement(finder);
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
            return new FactGrammar(() =>
                IsUrlMatch(Application.Driver.Url, toUrl(Application.Urls)),
                title);
        }

        protected static bool IsUrlMatch(string browserUrl, string url)
        {
            var browserUri = new Uri(browserUrl.ToAbsoluteUrl());
            var searchUri = new Uri(url.ToAbsoluteUrl());
            if (!browserUri.Host.Equals(searchUri.Host, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            return browserUri.AbsolutePath.StartsWith(searchUri.AbsolutePath, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}