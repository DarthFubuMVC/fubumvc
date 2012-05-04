using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Assets.Http;
using OpenQA.Selenium;
using Serenity.Endpoints;

namespace Serenity
{
    /// <summary>
    /// Called immediately after navigating the browser to a new url.
    /// Useful as a way to deal with login pages in a web application
    /// </summary>
    public interface IAfterNavigation
    {
        void AfterNavigation(IWebDriver driver, string desiredUrl);
    }

    public class NulloAfterNavigation : IAfterNavigation
    {
        public void AfterNavigation(IWebDriver driver, string desiredUrl)
        {
            // Do nothing!
        }
    }

    public class NavigationDriver
    {
        private readonly IApplicationUnderTest _application;
        private IAfterNavigation _afterNavigation = new NulloAfterNavigation();

        public NavigationDriver(IApplicationUnderTest application)
        {
            _application = application;
        }

        public IAfterNavigation AfterNavigation
        {
            get { return _afterNavigation; }
            set { _afterNavigation = value; }
        }

        public void NavigateTo(object target)
        {
            var url = _application.Urls.UrlFor(target);

            NavigateToUrl(url);
        }

        public void NavigateToUrl(string url)
        {
            Debug.WriteLine("Navigating to " + url);
            _application.Driver.Navigate().GoToUrl(url);

            _afterNavigation.AfterNavigation(_application.Driver, url);
        }

        public void NavigateTo<T>(Expression<Action<T>> expression)
        {
            var url = _application.Urls.UrlFor(expression, null);
            NavigateToUrl(url);
        }

        public string AssetUrlFor(string file)
        {
            return _application.RootUrl + ("/_content/" + file).Replace("//", "/");
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

    public class AssetTagsState
    {
        public IList<IWebElement> Scripts { get; set; }
        public IList<IWebElement> Styles { get; set; }
    }

    public class ScreenDriver
    {
        private readonly IWebDriver _browser;
        private readonly Lazy<IWebElement> _head;

        public ScreenDriver(IWebDriver browser)
        {
            _browser = browser;
            _head = new Lazy<IWebElement>(() => _browser.FindElement(By.TagName("head")));
        }

        private IWebElement head
        {
            get { return _head.Value; }
        }

        private IWebElement elementFor(string name)
        {
            return _browser.FindElement(By.Id(name)) ?? _browser.FindElement(By.Name(name));
        }

        public AssetTagsState GetAssetDeclarationsFromTheHead()
        {
            return new AssetTagsState
            {
                Scripts = head.FindElements(By.TagName("script")).ToList(),
                Styles = head.FindElements(By.TagName("link")).Where(x => x.IsCssLink()).ToList()
            };
        }

        public AssetTagsState GetAssetDeclarations()
        {
            return new AssetTagsState
            {
                Scripts = _browser.FindElements(By.TagName("script")).ToList(),
                Styles = _browser.FindElements(By.TagName("link")).Where(x => x.IsCssLink()).ToList()
            };
        }
    }
}