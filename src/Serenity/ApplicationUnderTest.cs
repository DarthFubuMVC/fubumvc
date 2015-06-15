using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;

namespace Serenity
{
    public class ApplicationUnderTest : IApplicationUnderTest
    {
        private readonly string _name;
        private readonly string _rootUrl;
        private readonly IBrowserLifecycle _browser;
        private readonly Lazy<IServiceFactory> _container;
        private readonly Lazy<IUrlRegistry> _urls;
        private readonly Lazy<IServiceLocator> _services;
        private readonly Lazy<NavigationDriver> _navigation;


        public ApplicationUnderTest(FubuRuntime runtime, ApplicationSettings settings, IBrowserLifecycle browser)
            : this(settings.Name, settings.RootUrl, browser, () => runtime.Factory)
        {
        }

        public ApplicationUnderTest(IApplicationSource source, ApplicationSettings settings, IBrowserLifecycle browser)
            : this(source.GetType().Name, settings.RootUrl, browser, () =>
            {
                var app = source.BuildApplication();
                return app.Bootstrap().Factory;
            })
        {
        }

        private ApplicationUnderTest(string name, string rootUrl, IBrowserLifecycle browser,
            Func<IServiceFactory> factorySource)
        {
            _name = name;
            _rootUrl = rootUrl;
            _browser = browser;

            _container = new Lazy<IServiceFactory>(factorySource);

            _urls = new Lazy<IUrlRegistry>(() => _container.Value.Get<IUrlRegistry>());

            _navigation = new Lazy<NavigationDriver>(() => new NavigationDriver(this));

            _services = new Lazy<IServiceLocator>(() => _container.Value.Get<IServiceLocator>());
        }

        public string Name
        {
            get { return _name; }
        }

        public string RootUrl
        {
            get { return _rootUrl; }
        }

        public IServiceLocator Services
        {
            get { return _services.Value; }
        }

        public IBrowserLifecycle Browser
        {
            get { return _browser; }
        }

        public IUrlRegistry Urls
        {
            get { return _urls.Value; }
        }


        public void Ping()
        {
            var client = new WebClient();
            if (_rootUrl != null) client.DownloadDataAsync(new Uri(_rootUrl));
        }

        public void Teardown()
        {
            _browser.SafeDispose();
        }

        public virtual NavigationDriver Navigation
        {
            get { return _navigation.Value; }
        }

        public virtual EndpointDriver Endpoints()
        {
            return new EndpointDriver(Urls, _rootUrl);
        }

        public IWebDriver Driver
        {
            get { return _browser.Driver; }
        }
    }
}