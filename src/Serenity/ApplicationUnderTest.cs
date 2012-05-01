using System;
using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Http;
using FubuMVC.Core.Urls;

using OpenQA.Selenium;
using Serenity.Endpoints;

namespace Serenity
{
    public class ApplicationUnderTest : IApplicationUnderTest
    {
        private readonly string _name;
        private readonly string _rootUrl;
        private Lazy<IWebDriver> _driver;
        private readonly Lazy<IContainerFacility> _container;
        private readonly Lazy<IUrlRegistry> _urls;
        private readonly Lazy<IServiceLocator> _services;
        private readonly Func<IWebDriver> _createWebDriver;


        public ApplicationUnderTest(FubuRuntime runtime, ApplicationSettings settings, Func<IWebDriver> createWebDriver)
            : this(settings.Name, settings.RootUrl, createWebDriver, () => runtime.Facility)
        {
            
        }

        public ApplicationUnderTest(IApplicationSource source, ApplicationSettings settings, Func<IWebDriver> createWebDriver)
            : this(source.GetType().Name, settings.RootUrl, createWebDriver, () =>
            {
                var app = source.BuildApplication();

                app.ModifyRegistry(r => r.Services(x =>
                {
                    x.ReplaceService<ICurrentHttpRequest>(new StubCurrentHttpRequest
                    {
                        ApplicationRoot = settings.RootUrl
                    });
                }));

                app.Bootstrap();


                return app.Facility;
            })
        {

        }

        private ApplicationUnderTest(string name, string rootUrl, Func<IWebDriver> createWebDriver, Func<IContainerFacility> containerSource)
        {
            _name = name;
            _rootUrl = rootUrl;

            _createWebDriver = createWebDriver;

            StartWebDriver();

            _container = new Lazy<IContainerFacility>(containerSource);

            _urls = new Lazy<IUrlRegistry>(() =>
            {
                var urls = GetInstance<IUrlRegistry>();
                urls.As<UrlRegistry>().RootAt(_rootUrl);
                return urls;
            });

            _services = new Lazy<IServiceLocator>(() => _container.Value.Get<IServiceLocator>());
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsDriverInUse
        {
            get { return _driver != null && _driver.IsValueCreated; }
        }

        public string RootUrl
        {
            get { return _rootUrl; }
        }

        public T GetInstance<T>()
        {
            return _container.Value.Get<T>();
        }

        public object GetInstance(Type type)
        {
            return _services.Value.GetInstance(type);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _container.Value.GetAll<T>();
        }

        public IUrlRegistry Urls
        {
            get { return _urls.Value; }
        }

        public void StartWebDriver()
        {
            _driver = new Lazy<IWebDriver>(_createWebDriver);
        }

        public void StopWebDriver()
        {
            if (!IsDriverInUse) return;

            Driver.Close();
            Driver.SafeDispose();
        }

        public void Ping()
        {
            var client = new WebClient();
            if (_rootUrl != null) client.DownloadDataAsync(new Uri(_rootUrl));
        }

        public void Teardown()
        {
            StopWebDriver();
        }

        public NavigationDriver Navigation
        {
            get { return new NavigationDriver(this); }
        }

        public EndpointDriver Endpoints()
        {
            return new EndpointDriver(Urls);
        }

        public IWebDriver Driver
        {
            get { return _driver.Value; }
        }
    }

    public class StubCurrentHttpRequest : ICurrentHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string ApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";
        public string StubFullUrl = "http://server/";

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string FullUrl()
        {
            return StubFullUrl;
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(ApplicationRoot);
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }
    }
}