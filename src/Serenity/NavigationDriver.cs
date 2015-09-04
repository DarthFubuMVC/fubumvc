using System;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;
using StoryTeller;

namespace Serenity
{
    public class NavigationDriver
    {
        private readonly IBrowserLifecycle _browserLifecycle;
        private readonly IUrlRegistry _urls;
        private readonly IAfterNavigation _afterNavigation = new NulloAfterNavigation();
        private readonly FubuRuntime _runtime;

        public NavigationDriver(IBrowserLifecycle browserLifecycle, IUrlRegistry urls, IAfterNavigation afterNavigation,
            FubuRuntime runtime)
        {
            _browserLifecycle = browserLifecycle;
            _urls = urls;
            _afterNavigation = afterNavigation;
            _runtime = runtime;
        }

        internal INavigationLogger Logger = new NulloNavigationLogger();

        public void NavigateTo<TInputModel>() where TInputModel : class
        {
            var url = _urls.UrlFor<TInputModel>();
            NavigateToUrl(url);
        }

        public void NavigateTo(object target)
        {
            var url = _urls.UrlFor(target, "GET");

            NavigateToUrl(url);
        }

        private string correctUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;

            if (url.StartsWith("~/"))
            {
                url = url.TrimStart('~');
            }

            return _runtime.BaseAddress.AppendUrl(url);
        }

        public void NavigateToUrl(string url)
        {
            url = correctUrl(url);

            var driver = _browserLifecycle.Driver;

            Logger.Navigating(url, () => driver.Navigate().GoToUrl(url));

            string title = "";

            try
            {
                title = driver.Title;
            }
            catch (Exception)
            {
                // don't throw here.
            }
            

            if (title == "Exception!")
            {
                var element = driver.FindElement(By.Id("error"));
                StoryTellerAssert.Fail(element == null ? driver.PageSource : element.Text);
            }


            _afterNavigation.AfterNavigation(driver, url);
        }

        public void NavigateTo<T>(Expression<Action<T>> expression)
        {
            var url = _urls.UrlFor(expression, "GET");
            NavigateToUrl(url);
        }

        public string AssetUrlFor(string file)
        {
            return _runtime.BaseAddress + ("/_content/" + file).Replace("//", "/");
        }

        public string GetCurrentUrl()
        {
            return Driver.Url;
        }

        public void NavigateToHome()
        {
            NavigateToUrl(_runtime.BaseAddress);
        }

        // 
        public IWebDriver Driver
        {
            get { return _browserLifecycle.Driver; }
        }

        public ScreenDriver GetCurrentScreen()
        {
            return new ScreenDriver(Driver);
        }

        public IAfterNavigation AfterNavigation
        {
            get { return _afterNavigation; }
        }
    }
}