using System;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Endpoints;
using HtmlTags;
using OpenQA.Selenium;
using StoryTeller;
using StoryTeller.Results;

namespace Serenity
{
    public class NavigationDriver
    {
        private readonly IApplicationUnderTest _application;
        private IAfterNavigation _afterNavigation = new NulloAfterNavigation();

        public NavigationDriver(IApplicationUnderTest application)
        {
            _application = application;
        }

        internal INavigationLogger Logger = new NulloNavigationLogger();

        public IAfterNavigation AfterNavigation
        {
            get { return _afterNavigation; }
            set { _afterNavigation = value; }
        }

        public void NavigateTo<TInputModel>() where TInputModel : class
        {
            var url = _application.Urls.UrlFor<TInputModel>();
            NavigateToUrl(url);
        }

        public void NavigateTo(object target)
        {
            var url = _application.Urls.UrlFor(target, categoryOrHttpMethod:"GET");

            NavigateToUrl(url);
        }

        private string correctUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;

            if (url.StartsWith("~/"))
            {
                url = url.TrimStart('~');
            }

            return _application.RootUrl.AppendUrl(url);
        }

        public void NavigateToUrl(string url)
        {
            url = correctUrl(url);

            var driver = _application.Driver;

            Logger.Navigating(url, () => driver.Navigate().GoToUrl(url));

            if (driver.Title == "Exception!")
            {
                var element = driver.FindElement(By.Id("error"));
                StoryTellerAssert.Fail(element == null ? driver.PageSource : element.Text);
            }


            _afterNavigation.AfterNavigation(driver, url);
        }

        public void NavigateTo<T>(Expression<Action<T>> expression)
        {
            var url = _application.Urls.UrlFor(expression, "GET");
            NavigateToUrl(url);
        }

        public string AssetUrlFor(string file)
        {
            return _application.RootUrl + ("/_content/" + file).Replace("//", "/");
        }

        public string GetCurrentUrl()
        {
            return Driver.Url;
        }

        public void NavigateToHome()
        {
            NavigateToUrl(_application.RootUrl);
        }

        // 
        public IWebDriver Driver
        {
            get
            {
                return _application.Driver;
            }
        }

        // TODO -- let's get rid of this
        public EndpointDriver GetEndpointDriver()
        {
            return new EndpointDriver(_application.Urls);
        }

        public ScreenDriver GetCurrentScreen()
        {
            return new ScreenDriver(Driver);
        }
    }
}